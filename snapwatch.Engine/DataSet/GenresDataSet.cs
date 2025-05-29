using System.Collections.Generic;

namespace snapwatch.Engine.DataSet
{
    public class GenresDataSet
    {
        protected Dictionary<uint, string> Genres = new Dictionary<uint, string>
        {
            { 18, "Драма" },
            { 36, "История" },
            { 27, "Ужасы" },
            { 16, "Анимация" },
            { 99, "Документалка" },
            { 10751, "Семейный" },
            { 10402, "Музыкальный" },
            { 9648, "Мистический" },
            { 878, "Научная фантастика" },
            { 28, "Боевик" },
            { 35, "Комедия" },
            { 80, "Криминал" },
            { 14, "Фантастика" },
            { 37, "Западный" },
            { 12, "Приключенческий" },
            { 10749, "Роман" },
            { 10770, "Сериал" },
            { 53, "Триллер" },
            { 10752, "Война" },
        };

        protected string GetGenreById(uint id)
        {
            if (this.Genres.TryGetValue(id, out var genre))
            {
                return genre;
            }

            return "Неизвестно";
        }
    }
}
