using System.Windows.Media.Imaging;

namespace snapwatch.Core.Interface
{
    public interface ICacheRepository
    {
        /// <summary>
        /// получения из кеша изображение
        /// </summary>
        /// <param name="url">URL картинки</param>
        BitmapImage Get_ImageCache(string url);

        /// <summary>
        /// добавления в кеш изображения
        /// </summary>
        /// <param name="url">URL картинки(ключ)</param>
        /// <param name="image">само изображения</param>
        void Add_ImageCache(string url, BitmapImage image);
    }
}
