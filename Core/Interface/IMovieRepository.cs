using snapwatch.Core.Models;
using System.Collections.Generic;
using System.Windows.Documents;

namespace snapwatch.Core.Interface
{
    public interface IMovieRepository
    {
        MoviesModel GetMovies();

        List<MovieModel> GetMoviesByTone(string tone);
    }
}
