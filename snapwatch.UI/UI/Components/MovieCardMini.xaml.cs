using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using snapwatch.Core.Core;
using System.Windows.Media;
using snapwatch.Core.Service;

namespace snapwatch.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для MovieCardMini.xaml
    /// </summary>
    public partial class MovieCardMini : UserControl
    {
        /// <summary>
        /// Классы и методы
        /// </summary>
        private readonly Config _config;
        private readonly HttpConfig _httpConfig;

        private bool _imageLoaded = false;

        public MovieCardMini()
        {
            InitializeComponent();
            this._config = new Config();
            this._httpConfig = new HttpConfig();
        }

        private void MovieCardMini_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsVisible && !this._imageLoaded && !string.IsNullOrEmpty(PosterPath))
            {
                this._imageLoaded = true;
                LoadImageAsync(PosterPath);
            }
        }

        private void MovieCardMini_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && !this._imageLoaded && !string.IsNullOrEmpty(PosterPath))
            {
                this._imageLoaded = true;
                LoadImageAsync(PosterPath);
            }
        }

        /// <summary>
        /// Загрузка постеров
        /// </summary>
        /// <param name="path">путь изображения</param>
        private async void LoadImageAsync(string path)
        {
            var preloaderUri = new Uri("pack://application:,,,/snapwatch.UI/Public/images/image_preloader.png");
            var defaultUri = new Uri("pack://application:,,,/snapwatch.UI/Public/images/default_image.jpg");

            MovieBrash.ImageSource = new BitmapImage(preloaderUri);

            try
            {
                string url = $"https://image.tmdb.org/t/p/w500{path}?api_key={this._config.ReturnConfig().API_KEY_TMDB}";

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

                    MovieBrash.ImageSource = bitmap;
                }
                else
                {
                    MovieBrash.ImageSource = new BitmapImage(defaultUri);
                }
            }
            catch
            {
                MovieBrash.ImageSource = new BitmapImage(defaultUri);
            }
        }

        private static void OnPosterPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = (MovieCardMini)d;
            if (card.IsVisible && !card._imageLoaded && e.NewValue is string newPath && !string.IsNullOrEmpty(newPath))
            {
                card._imageLoaded = true;
                card.LoadImageAsync(newPath);
            }
        }

        /// <summary>
        /// Title Dependency
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MovieCardMini), new PropertyMetadata(string.Empty));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// PosterPath Dependency
        /// </summary>
        public static readonly DependencyProperty PosterPathProperty = DependencyProperty.Register("PosterPath", typeof(string), typeof(MovieCardMini), new PropertyMetadata(null, OnPosterPathChanged));
        public string PosterPath
        {
            get { return (string)GetValue(PosterPathProperty); }
            set { SetValue(PosterPathProperty, value); }
        }

        public ScaleTransform ScaleTransform => scaleTransform;
    }
}
