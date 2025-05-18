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
using System.Text.Json;
using System.Threading.Tasks;

namespace snapwatch.Core.Repository
{
    public class MovieRepository : ToneDataSet, IMovieRepository
    {
        private readonly Config _config;
        private readonly UIException _uiException;
        private readonly IndexService _indexService;
        private readonly TranslateService _translateService;

        private readonly ToneBuilder _toneBuilder;

        private readonly short MAX_COUNT_MOVIES = 500;
        private readonly Dictionary<ushort, uint> _pidx;

        protected List<MoviesModel> _moviesByCache = null;

        public MovieRepository()
        {
            this._config = new Config();
            this._uiException = new UIException();
            this._indexService = new IndexService();
            this._translateService = new TranslateService();

            this._toneBuilder = new ToneBuilder();

            this._pidx = this._indexService.LoadPIDX();
        }

        public MoviesModel GetMovies()
        {
            StreamReader sr = null;
            FileStream fileSt = null;

            try
            {
                var r = new Random();
                ushort randomPage = (ushort)r.Next(1, MAX_COUNT_MOVIES + 1);

                //if (!this._pidx.TryGetValue(randomPage, out var offset))
                //{
                //    return null;
                //}

                //uint nextOffset = this._indexService.GetNextOffset(randomPage);
                //if (nextOffset <= offset)
                //{
                //    return null;
                //}

                //int length = (int)(nextOffset - offset);

                //fileSt = new FileStream(this._config.ReturnConfig().MOVIES_JSON_READ, FileMode.Open, FileAccess.Read);
                //fileSt.Seek(offset, SeekOrigin.Begin);

                //byte[] buffer = new byte[length];
                //int bytesRead = fileSt.Read(buffer, 0, length);
                //if (bytesRead != length)
                //{
                //    return null;
                //}

                //var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                //MoviesModel movies = JsonSerializer.Deserialize<MoviesModel>(buffer, jsonOptions);

                //if (movies != null && movies.Page == randomPage)
                //{
                //    return movies;
                //}

                if (this._moviesByCache == null)
                {
                    string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);
                    this._moviesByCache = JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);
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

        public List<MovieModel> GetMoviesByTone(string tone)
        {
            List<MovieModel> moviesByTone = [];

            try
            {
                if (this._moviesByCache == null)
                {
                    string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);
                    this._moviesByCache = JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);
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

        public Task<List<MovieModel>> GetMoviesByToneAsync(string tone)
        {
            return Task.Run(() => this.GetMoviesByTone(tone));
        }
    }
}
