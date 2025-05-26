using snapwatch.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace snapwatch.Core.Interface
{
    public interface IMovieRepository
    {
        /// <summary>
        /// Получение фильмов по (rand)pages
        /// </summary>
        MoviesModel GetMovies();

        /// <summary>
        /// получение фильмов по эмоциональной тональности
        /// </summary>
        /// <param name="tone">тональность для поиска</param>
        List<MovieModel> GetMoviesByTone(string tone);

        /// <summary>
        /// получение фильмов по эмоциональной тональности (асинхронное)
        /// </summary>
        /// <param name="tone">тональность для поиска</param>
        Task<List<MovieModel>> GetMoviesByToneAsync(string tone);

        /// <summary>
        /// простой и быстрый поиск фильмов по косинусного сравнения
        /// </summary>
        /// <param name="text">текст написанный пользователем</param>
        Task<List<MovieModel>> GetMoviesByText_Simple(string text);

        /// <summary>
        /// сложный, медленный поиск фильмов по LSA/SVD алгоритмам
        /// </summary>
        /// <param name="text">текст написанный пользователем</param>
        Task<List<MovieModel>> GetMoviesByText_HardAsync(string text);

        /// <summary>
        /// вывод информации о фильме по ID
        /// </summary>
        /// <param name="id">id фильма</param>
        MovieModel GetMovieByID(uint id);

        /// <summary>
        /// получение жанров и форматирование
        /// </summary>
        /// <param name="movie">фильм</param>
        string GetGenreByMovie(MovieModel movie);
    }
}
