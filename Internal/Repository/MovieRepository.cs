using snapwatch.Internal.Core;
using snapwatch.Internal.Interface;
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

        private readonly short MAX_COUNT_MOVIES = 500;

        public MovieRepository()
        {
            this._config = new Config();
        }

        public MoviesModel GetMovies()
        {
            try
            {
                string movieFile = File.ReadAllText(this._config.ReturnConfig().MOVIES_JSON_READ);

                List<MoviesModel> moviesJson = JsonSerializer.Deserialize<List<MoviesModel>>(movieFile);

                if (moviesJson == null || moviesJson.Count == 0)
                {
                    return null;
                }

                var r = new Random();
                ushort randomPage = (ushort) r.Next(1, MAX_COUNT_MOVIES + 1);

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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
