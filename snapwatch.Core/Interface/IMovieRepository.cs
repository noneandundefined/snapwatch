using snapwatch.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace snapwatch.Core.Interface
{
    public interface IMovieRepository
    {
        MoviesModel GetMovies();

        List<MovieModel> GetMoviesByTone(string tone);

        Task<List<MovieModel>> GetMoviesByToneAsync(string tone);
    }
}
