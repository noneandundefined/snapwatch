using Newtonsoft.Json;
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

namespace snapwatch.Core.Repository
{
    public class MovieRepository : ToneDataSet, IMovieRepository
    {
        /// <summary>
        /// Классы и методы
        /// </summary>
        private readonly Config _config;
        private readonly UIException _uiException;
        private readonly IndexService _indexService;
        private readonly TranslateService _translateService;
        private readonly HttpClient _httpClient;

        private readonly LSABuilder _lsaBuilder;
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
        /// получение фильмов по эмоциональной тональности
        /// </summary>
        /// <param name="tone">тональность для поиска</param>
        public List<MovieModel> GetMoviesByTone(string tone)
        {
            List<MovieModel> moviesByTone = [];

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
                    "anticipation" => AnticipationGenresID,
                    "joy" => JoyGenresID,
                    "trust" => TrustGenresID,
                    "sadness" => SadnessGenresID,
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
        public Task<List<MovieModel>> GetMoviesByToneAsync(string tone)
        {
            return Task.Run(() => this.GetMoviesByTone(tone));
        }

        /// <summary>
        /// простой и быстрый поиск фильмов по косинусного сравнения
        /// </summary>
        /// <param name="text">текст написанный пользователем</param>
        public Task<List<MovieModel>> GetMoviesByText_Simple(string text)
        {
            return Task.Run(() =>
            {
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

                    List<MovieModel> filteredMovies = this._moviesByCache.AsParallel().
                                                    WithDegreeOfParallelism(Environment.ProcessorCount).
                                                    SelectMany(group => group.Results).ToList();

                    var movies = this._lsaBuilder.TFIDF_Cosine(filteredMovies, text);

                    return movies.Select(movie => movie.movies).ToList();
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
        public Task<List<MovieModel>> GetMoviesByText_HardAsync(string text)
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

                    return System.Text.Json.JsonSerializer.Deserialize<List<MovieModel>>(result);
                }
                catch (Exception ex)
                {
                    this._uiException.Error(ex.Message, "Ошибка поиска фильмов по запросу");
                    return null;
                }
            });
        }
    }
}
