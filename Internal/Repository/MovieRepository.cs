using snapwatch.Internal.Core;
using snapwatch.Internal.Interface;
using snapwatch.Internal.Service;
using snapwatch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace snapwatch.Internal.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly Config _config;
        private readonly UIException _uiException;
        private readonly IndexService _indexService;

        private readonly short MAX_COUNT_MOVIES = 500;
        private readonly Dictionary<ushort, uint> _pidx;

        public MovieRepository()
        {
            this._config = new Config();
            this._uiException = new UIException();
            this._indexService = new IndexService();

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

                if (!this._pidx.TryGetValue(randomPage, out var offset))
                {
                    return null;
                }

                fileSt = new FileStream(this._config.ReturnConfig().MOVIES_JSON_READ, FileMode.Open, FileAccess.Read);
                fileSt.Seek(offset, SeekOrigin.Begin);

                sr = new StreamReader(fileSt);

                char[] buffer = new char[200000];
                int readChar = sr.Read(buffer, 0, buffer.Length);
                string jsonChunk = new string(buffer, 0, readChar);

                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                MoviesModel movies = JsonSerializer.Deserialize<MoviesModel>(jsonChunk, jsonOptions);

                if (movies != null && movies.Page == randomPage)
                {
                    return movies;
                }

                //string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);

                //List<MoviesModel> moviesJson = JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);

                //if (moviesJson == null || moviesJson.Count == 0)
                //{
                //    return null;
                //}

                //foreach (var movies in moviesJson)
                //{
                //    if (movies.Page == randomPage)
                //    {
                //        return movies;
                //    }
                //}

                return null;
            }
            catch (Exception ex)
            {
                this._uiException.Error(ex.Message, "Error get movies");
                return null;
            }
            finally
            {
                sr?.Dispose();
                fileSt?.Dispose();
            }
        }
    }
}
