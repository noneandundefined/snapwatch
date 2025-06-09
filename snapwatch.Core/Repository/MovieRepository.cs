using snapwatch.Core.Core;
using snapwatch.Core.Interface;
using snapwatch.Core.Models;
using snapwatch.Core.Service;
using snapwatch.Engine;
using snapwatch.Engine.DataSet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using snapwatch.Core.Utilities;

namespace snapwatch.Core.Repository
{
    public class MovieRepository : GenresDataSet, IMovieRepository
    {
        /// <summary>
        /// Классы и методы
        /// </summary>
        private readonly Config _config;
        private readonly UIException _uiException;
        private readonly IndexService _indexService;
        private readonly TranslateService _translateService;
        private readonly HttpClient _httpClient;
        private readonly HttpConfig _httpConfig;

        private readonly ToneDataSet _toneDataSet;

        private readonly LSABuilder _lsaBuilder = App._lsaBuilder;
        private readonly ToneBuilder _toneBuilder;

        /// <summary>
        /// максимальное кол-во pages фильмов
        /// </summary>
        private readonly short MAX_COUNT_MOVIES = 500;

        /// <summary>
        /// словарь: страница -> смещение (индексации фильмов)
        /// </summary>
        private readonly Dictionary<ushort, uint> _pidx;

        /// <summary>
        /// кешированный список фильмов
        /// </summary>
        protected List<MoviesModel> _moviesByCache = null;

        public MovieRepository()
        {
            this._config = new Config();
            this._uiException = new UIException();
            this._indexService = new IndexService();
            this._translateService = new TranslateService();
            this._httpClient = new HttpClient();
            this._httpConfig = new HttpConfig();

            this._toneDataSet = new ToneDataSet();

            this._lsaBuilder = new LSABuilder();
            this._toneBuilder = new ToneBuilder();

            this._pidx = this._indexService.LoadPIDX();
        }

        /// <summary>
        /// Получение фильмов по (rand)pages
        /// </summary>
        public MoviesModel GetMovies()
        {
            StreamReader sr = null;
            FileStream fileSt = null;

            try
            {
                var r = new Random();
                ushort randomPage = (ushort)r.Next(1, MAX_COUNT_MOVIES + 1);

                if (this._moviesByCache == null)
                {
                    string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);
                    this._moviesByCache = System.Text.Json.JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);
                }

                if (this._moviesByCache == null || this._moviesByCache.Count == 0)
                {
                    throw new Exception("Ошибка чтения файла (json) с фильмами.");
                }

                foreach (var movies in this._moviesByCache)
                {
                    if (movies.Page == randomPage)
                    {
                        return movies;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._uiException.Error(ex.Message, "Ошибка получения фильмов");
                return null;
            }
            finally
            {
                sr?.Dispose();
                fileSt?.Dispose();
            }
        }

        /// <summary>
        /// получения всех фильмов в файле
        /// </summary>
        public List<MoviesModel> GetDataFileMovie()
        {
            if(this._moviesByCache == null)
            {
                string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);
                this._moviesByCache = System.Text.Json.JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);
            }

            return this._moviesByCache;
        }

        /// <summary>
        /// получение фильмов по эмоциональной тональности
        /// </summary>
        /// <param name="tone">тональность для поиска</param>
        public HashSet<MovieModel> GetMoviesByTone(string tone)
        {
            HashSet<MovieModel> moviesByTone = [];

            try
            {
                if (this._moviesByCache == null)
                {
                    string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);
                    this._moviesByCache = System.Text.Json.JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);
                }

                if (this._moviesByCache == null || this._moviesByCache.Count == 0)
                {
                    throw new Exception("Ошибка чтения файла (json) с фильмами.");
                }

                HashSet<ushort> isGenres = tone.ToLower() switch
                {
                    "anticipation" => this._toneDataSet.AnticipationGenresID,
                    "joy" => this._toneDataSet.JoyGenresID,
                    "trust" => this._toneDataSet.TrustGenresID,
                    "sadness" => this._toneDataSet.SadnessGenresID,
                    _ => throw new ArgumentException("Неправильно указан тон."),
                };

                var filteredMovies = this._moviesByCache.AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                    .SelectMany(group => group.Results
                    .Where(movie => movie.GenreIds != null && movie.GenreIds.Any(id => isGenres.Contains(id))))
                    .ToList();

                var r = new Random();
                int startIndex = r.Next(filteredMovies.Count);

                object syncLock = new();

                Parallel.ForEach(Enumerable.Range(0, filteredMovies.Count), new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, (i, state) =>
                {
                    int index = (startIndex + i) % filteredMovies.Count;
                    var movie = filteredMovies[index];

                    string overview = movie.Overview;
                    string toneMovie = this._toneBuilder.Tone(overview); // anticipation | joy | trust | sadness

                    if (toneMovie == tone)
                    {
                        lock (syncLock)
                        {
                            if (moviesByTone.Count < 25)
                            {
                                moviesByTone.Add(movie);

                                if (moviesByTone.Count >= 25)
                                {
                                    state.Stop();
                                }
                            }
                        }
                    }
                });

                return moviesByTone;
            }
            catch (Exception ex)
            {
                this._uiException.Error(ex.Message, "Ошибка получения фильмов");
                return null;
            }
        }

        /// <summary>
        /// получение фильмов по эмоциональной тональности (асинхронное)
        /// </summary>
        /// <param name="tone">тональность для поиска</param>
        public Task<HashSet<MovieModel>> GetMoviesByToneAsync(string tone)
        {
            return Task.Run(() => this.GetMoviesByTone(tone));
        }

        /// <summary>
        /// простой и быстрый поиск фильмов по косинусного сравнения
        /// </summary>
        /// <param name="text">текст написанный пользователем</param>
        public Task<HashSet<MovieModel>> GetMoviesByText_Simple(string text)
        {
            return Task.Run(async () =>
            {
                string prepareText = text;

                try
                {
                    if(!this._translateService.IS_EN(text))
                    {
                        prepareText = await this._translateService.RU_TO_EN(text);
                    }

                    if (this._moviesByCache == null)
                    {
                        string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);
                        this._moviesByCache = System.Text.Json.JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);
                    }

                    if (this._moviesByCache == null || this._moviesByCache.Count == 0)
                    {
                        throw new Exception("Ошибка чтения файла (json) с фильмами.");
                    }

                    List<MovieModel> filteredMovies = this._moviesByCache.Shuffle().AsParallel().
                                                    WithDegreeOfParallelism(Environment.ProcessorCount).
                                                    SelectMany(group => group.Results).ToList();

                    var movies = this._lsaBuilder.TFIDF_Cosine_Overviews(filteredMovies, prepareText);

                    return movies.Select(movie => movie.movies).ToHashSet();
                }
                catch (Exception ex)
                {
                    this._uiException.Error(ex.Message, "Ошибка получения фильмов");
                    return null;
                }
            });
        }

        /// <summary>
        /// сложный, медленный поиск фильмов по LSA/SVD алгоритмам
        /// </summary>
        /// <param name="text">текст написанный пользователем</param>
        public Task<HashSet<MovieModel>> GetMoviesByText_HardAsync(string text)
        {
            return Task.Run(async () =>
            {
                string prepareText = text;

                try
                {
                    if (!this._translateService.IS_EN(text))
                    {
                        prepareText = await this._translateService.RU_TO_EN(text);
                    }

                    string jsonPayload = "{\"text\": \"" + prepareText + "\"";

                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await this._httpClient.PostAsync(_config.ReturnConfig().SERVER_API_ADDRESS, content);

                    string result = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(result);
                    }

                    return System.Text.Json.JsonSerializer.Deserialize<HashSet<MovieModel>>(result);
                }
                catch (Exception ex)
                {
                    this._uiException.Error(ex.Message, "Ошибка поиска фильмов по запросу");
                    return null;
                }
            });
        }

        /// <summary>
        /// вывод информации о фильме по ID
        /// </summary>
        /// <param name="id">id фильма</param>
        public MovieModel GetMovieByID(uint id)
        {
            try
            {
                if(this._moviesByCache == null)
                {
                    string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);
                    this._moviesByCache = System.Text.Json.JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);
                }

                if(this._moviesByCache == null || this._moviesByCache.Count == 0)
                {
                    throw new Exception("Ошибка чтения файла (json) с фильмами.");
                }

                foreach (var movies in this._moviesByCache)
                {
                    foreach (var movie in movies.Results)
                    {
                        if (movie.Id == id)
                        {
                            return movie;
                        }
                    }
                }

                return null;
            }
            catch(Exception ex)
            {
                this._uiException.Error(ex.Message, "Ошибка поиска фильмов по запросу");
                return null;
            }
        }

        /// <summary>
        /// получения видео(трейлера) и доп. информации фильма
        /// </summary>
        /// <param name="id">id фильма</param>
        public async Task<VideoMovieModel> GetVideoMovie(uint id)
        {
            string url = $"https://api.themoviedb.org/3/movie/{id}/videos?api_key={this._config.ReturnConfig().API_KEY_TMDB}";

            try
            {
                var handler = new HttpClientHandler
                {
                    Proxy = this._httpConfig.GetProxy(),
                    UseProxy = true
                };

                using var httpClient = new HttpClient(handler);
                httpClient.Timeout = TimeSpan.FromMinutes(1);

                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();

                return System.Text.Json.JsonSerializer.Deserialize<VideoMovieModel>(content);
            }
            catch (Exception ex)
            {
                this._uiException.Error(ex.Message, "Ошибка получения трейлера фильма.");
                return null;
            }
        }

        /// <summary>
        /// получение жанров и форматирование
        /// </summary>
        /// <param name="movie">фильм</param>
        public string GetGenreByMovie(MovieModel movie)
        {
            List<string> genres = [];

            foreach (var genre in movie.GenreIds)
            {
                genres.Add(GetGenreById(genre));
            }

            return genres.Aggregate((current, next) => $"{current} / {next}");
        }
    }
}
