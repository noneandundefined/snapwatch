using snapwatch.Core.Core;
using snapwatch.Core.Interface;
using snapwatch.Core.Models;
using snapwatch.Core.Repository;
using snapwatch.Core.Service;
using snapwatch.Engine.DataSet;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace snapwatch
{
    /// <summary>
    /// Логика взаимодействия для DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Классы и методы
        /// </summary>
        private readonly Config _config;
        private readonly HttpConfig _httpConfig;
        private readonly IMovieRepository _movieRepository;

        private MovieModel _movie;
        private uint _movieID;

        public DetailsWindow(uint ID)
        {
            InitializeComponent();
            this._config = new Config();
            this._httpConfig = new HttpConfig();
            this._movieRepository = new MovieRepository();

            this._movieID = ID;
            this._movie = this._movieRepository.GetMovieByID(ID);

            DataContext = this;
        }

        private string title = "";
        public string TitleMovie
        {
            get => this.title;
            set
            {
                this.title = value; 
                OnPropertyChanged();
            }
        }

        private string description = "";
        public string DescriptionMovie
        {
            get => this.description;
            set
            {
                this.description = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Загрузка постеров
        /// </summary>
        /// <param name="path">путь изображения</param>
        private async void LoadImageAsync(string path, object xName)
        {
            var preloaderUri = new Uri("pack://application:,,,/snapwatch.UI/Public/images/image_preloader.png");
            var defaultUri = new Uri("pack://application:,,,/snapwatch.UI/Public/images/default_image.jpg");

            this.SetImageSource(xName, new BitmapImage(preloaderUri));

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Иконки
            this.LoadImageAsync(this._movie.BackdropPath, BackgroundImage);
            this.LoadImageAsync(this._movie.PosterPath, PosterImageBrash);

            // Текстовые данные
            this.TitleMovie = this._movie.Title;
            this.DescriptionMovie = this._movie.Overview;

            // Жанр

        }
    }
}
