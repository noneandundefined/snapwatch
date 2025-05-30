using snapwatch.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace snapwatch.Core.Repository
{
    public class CacheRepository : ICacheRepository
    {
        /// <summary>
        /// максимальное кол-во изображений в кеше
        /// </summary>
        private const int _maxCacheCountImage = 100;

        /// <summary>
        /// хранит соответствие URL -> индекс
        /// </summary>
        private readonly Dictionary<string, int> _cacheIndexImage = [];

        /// <summary>
        /// кеш изображений
        /// </summary>
        private readonly Dictionary<int, BitmapImage> _cacheImage = []; // max=100

        /// <summary>
        /// каждый новый элемент идёт в индекс currentIndex % 100
        /// </summary>
        private int _currentIndex = 0;

        /// <summary>
        /// получения из кеша изображение
        /// </summary>
        /// <param name="url">URL картинки</param>
        public BitmapImage Get_ImageCache(string url)
        {
            if (this._cacheIndexImage.TryGetValue(url, out int index))
            {
                if(this._cacheImage.TryGetValue(index, out BitmapImage image))
                {
                    return image;
                }
            }

            return null;
        }

        /// <summary>
        /// добавления в кеш изображения
        /// </summary>
        /// <param name="url">URL картинки(ключ)</param>
        /// <param name="image">само изображения</param>
        public void Add_ImageCache(string url, BitmapImage image)
        {
            if (this._cacheIndexImage.ContainsKey(url))
            {
                return;
            }

            int index = this._currentIndex % _maxCacheCountImage;

            string existingURL = this._cacheIndexImage.FirstOrDefault(kv => kv.Value == index).Key;
            if (existingURL != null)
            {
                this._cacheIndexImage.Remove(existingURL);
            }

            this._cacheImage[index] = image;
            this._cacheIndexImage[url] = index;

            _currentIndex++;
        }
    }
}
