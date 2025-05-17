using snapwatch.Core.Core;
using snapwatch.Core.Interface;
using snapwatch.Core.Models;
using snapwatch.Core.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace snapwatch.Core.Repository
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

                string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);

                List<MoviesModel> moviesJson = JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);

                if (moviesJson == null || moviesJson.Count == 0)
                {
                    return null;
                }

                foreach (var movies in moviesJson)
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
                this._uiException.Error(ex.Message, "Error get movies");
                return null;
            }
            finally
            {
                sr?.Dispose();
                fileSt?.Dispose();
            }
        }

        public MoviesModel GetMoviesByTone(string tone)
        {
            
        }
    }
}
