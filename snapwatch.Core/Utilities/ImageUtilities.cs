using snapwatch.Core.Core;
using snapwatch.Core.Interface;
using snapwatch.Core.Service;
using System;
using System.IO;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace snapwatch.Core.Utilities
{
    public class ImageUtilities
    {
        private readonly Config _config;
        private readonly HttpConfig _httpConfig;
        private readonly ICacheRepository _cacheRepository = App._cacheRepository;

        public ImageUtilities()
        {
            this._config = new Config();
            this._httpConfig = new HttpConfig();
        }

        /// <summary>
        /// Загрузка постеров
        /// </summary>
        /// <param name="path">путь изображения</param>
        public async void LoadImageAsync(string path, object xName)
        {
            var preloaderUri = new Uri("pack://application:,,,/snapwatch.UI/Public/images/image_preloader.png");
            var defaultUri = new Uri("pack://application:,,,/snapwatch.UI/Public/images/default_image.jpg");

            this.SetImageSource(xName, new BitmapImage(preloaderUri));

            try
            {
                string url = $"https://image.tmdb.org/t/p/w500{path}?api_key={this._config.ReturnConfig().API_KEY_TMDB}";

                // попытка получить картинку из кеша
                var cachedImage = this._cacheRepository.Get_ImageCache(url);
                if(cachedImage != null)
                {
                    this.SetImageSource(xName, cachedImage);
                    return;
                }

                var handler = new HttpClientHandler
                {
                    Proxy = this._httpConfig.GetProxy(),
                    UseProxy = true
                };

                using var httpClient = new HttpClient(handler);
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var response = await httpClient.GetAsync(url);

                if(response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                    var bitmap = new BitmapImage();
                    using(var stream = new MemoryStream(imageBytes))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }

                    // добавления изображения в кеш
                    this._cacheRepository.Add_ImageCache(url, bitmap);

                    this.SetImageSource(xName, bitmap);
                }
                else
                {
                    this.SetImageSource(xName, new BitmapImage(defaultUri));
                }
            }
            catch
            {
                this.SetImageSource(xName, new BitmapImage(defaultUri));
            }
        }

        private void SetImageSource(object target, ImageSource source)
        {
            switch(target)
            {
                case Image image:
                    image.Source = source;
                    break;
                case ImageBrush brush:
                    brush.ImageSource = source;
                    break;
            }
        }
    }
}
