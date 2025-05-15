using snapwatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snapwatch.Internal.Interface
{
    public interface IMovieRepository
    {
        List<MoviesModel> GetMovies();
    }
}
