using snapwatch.Internal.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace snapwatch.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для MovieCard.xaml
    /// </summary>
    public partial class MovieCard : UserControl
    {
        private readonly Config _config;

        public MovieCard()
        {
            InitializeComponent();
            this._config = new Config();
        }

        /// <summary>
        /// Проверка элемента в видимости
        /// </summary>
        private void LoadVisibleImages()
        {
        }

        /// <summary>
        /// Загрузка постеров
        /// </summary>
        /// <param name="path">путь изображения</param>
        private async void LoadImageAsync(string path)
        {
            MovieBrash.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Public/images/image_preloader.png"));

            using (var ctx = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        string url = $"https://image.tmdb.org/t/p/w500{path}?api_key={this._config.ReturnConfig().API_KEY_TMDB}";

                        var response = await httpClient.GetAsync(url, ctx.Token);

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
                    }
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception)
                {

                }

                MovieBrash.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Public/images/default_image.jpg"));
            }
        }

        private static void OnPosterPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (MovieCard)d;
            if (e.NewValue != null)
            {
                window.LoadImageAsync(e.NewValue.ToString());
            }
            else
            {
                return;
            }
        }

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
