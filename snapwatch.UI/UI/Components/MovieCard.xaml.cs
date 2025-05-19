using snapwatch.Core.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace snapwatch.UI.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для MovieCard.xaml
    /// </summary>
    public partial class MovieCard : UserControl
    {
        private readonly Config _config;

        private bool _imageLoaded = false;

        public MovieCard()
        {
            InitializeComponent();
            this._config = new Config();

            this.Loaded += MovieCard_Loaded;
            this.IsVisibleChanged += MovieCard_IsVisibleChanged;
        }

        private void MovieCard_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsVisible && !this._imageLoaded && !string.IsNullOrEmpty(PosterPath))
            {
                this._imageLoaded = true;
                LoadImageAsync(PosterPath);
            }
        }

        private void MovieCard_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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
            MovieBrash.ImageSource = new BitmapImage(new Uri("pack://application:,,,/snapwatch.UI/Public/images/image_preloader.png"));

            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(1);

                string url = $"https://image.tmdb.org/t/p/w500{path}?api_key={this._config.ReturnConfig().API_KEY_TMDB}";

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                    var bitmap = new BitmapImage();
                    using (var memoryStream = new MemoryStream(imageBytes))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = memoryStream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }

                    MovieBrash.ImageSource = bitmap;
                    return;
                } 
                else
                {
                    MovieBrash.ImageSource = new BitmapImage(new Uri("pack://application:,,,/snapwatch.UI/Public/images/default_image.jpg"));
                }
            }
            catch (TaskCanceledException)
            {
                MovieBrash.ImageSource = new BitmapImage(new Uri("pack://application:,,,/snapwatch.UI/Public/images/default_image.jpg"));
            }
            catch (HttpRequestException)
            {
                MovieBrash.ImageSource = new BitmapImage(new Uri("pack://application:,,,/snapwatch.UI/Public/images/default_image.jpg"));
            }
            catch (Exception)
            {
                MovieBrash.ImageSource = new BitmapImage(new Uri("pack://application:,,,/snapwatch.UI/Public/images/default_image.jpg"));
            }
        }

        private static void OnPosterPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = (MovieCard)d;
            if (card.IsVisible && !card._imageLoaded && e.NewValue is string newPath && !string.IsNullOrEmpty(newPath))
            {
                card._imageLoaded = true;
                card.LoadImageAsync(newPath);
            }
        }

        // private static void OnPosterPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {}

        /// <summary>
        /// Title Dependency
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MovieCard), new PropertyMetadata(string.Empty));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// PosterPath Dependency
        /// </summary>
        public static readonly DependencyProperty PosterPathProperty = DependencyProperty.Register("PosterPath", typeof(string), typeof(MovieCard), new PropertyMetadata(null, OnPosterPathChanged));
        public string PosterPath
        {
            get { return (string)GetValue(PosterPathProperty); }
            set { SetValue(PosterPathProperty, value); }
        }

        public ScaleTransform ScaleTransform => scaleTransform;
    }
}
