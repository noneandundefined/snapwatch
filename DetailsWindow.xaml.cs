using snapwatch.Core.Core;
using snapwatch.Core.Interface;
using snapwatch.Core.Models;
using snapwatch.Core.Service;
using snapwatch.Core.Utilities;
using snapwatch.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
        private readonly MouseUtilities _mouseUtilities;
        private readonly ImageUtilities _imageUtilities;
        private readonly IMovieRepository _movieRepository = App._movieRepository;
        private readonly ICacheRepository _cacheRepository = App._cacheRepository;
        private readonly LSABuilder _lsaBuilder = App._lsaBuilder;

        private MovieModel _movie;
        private uint _movieID;

        public DetailsWindow(uint ID)
        {
            InitializeComponent();

            this._config = new Config();
            this._httpConfig = new HttpConfig();
            this._mouseUtilities = new MouseUtilities();
            this._imageUtilities = new ImageUtilities();

            this._movieID = ID;
            this._movie = this._movieRepository.GetMovieByID(ID);

            DataContext = this;

            this.GetSimilar();
        }

        /// <summary>
        /// Получение похожих фильмов
        /// </summary>
        private void GetSimilar()
        {
            List<MoviesModel> movies = this._movieRepository.GetDataFileMovie();

            List<MovieModel> fillteredMovies = movies.AsParallel().SelectMany(g => g.Results).ToList();

            var similars = this._lsaBuilder.TFIDF_Cosine_Overviews(fillteredMovies, this._movie.Overview);
            this.SimilarMovies = similars.Select(group => group.movies).ToHashSet();
        }

        /// <summary>
        /// Получение и вывод в UI id фильма
        /// </summary>
        private uint idMovie = 0;
        public uint IdMovie
        {
            get => this.idMovie;
            set
            {
                this.idMovie = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Получение и вывод в UI название фильма
        /// </summary>
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

        /// <summary>
        /// Получение и вывод в UI описания фильма
        /// </summary>
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
        /// Получение и вывод в UI жанры фильма
        /// </summary>
        private string genre = "";
        public string GenreMovie
        {
            get => this.genre;
            set
            {
                this.genre = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Получение и вывод в UI похожии фильмы
        /// </summary>
        private HashSet<MovieModel> similarMovies = [];
        public HashSet<MovieModel> SimilarMovies
        {
            get => this.similarMovies;
            set
            {
                this.similarMovies = value;
                OnPropertyChanged(nameof(SimilarMovies));
                OnPropertyChanged(nameof(HasNoMovies));
            }
        }

        /// <summary>
        /// Проверка на пустой ли список с фильмами
        /// </summary>
        public bool HasNoMovies => this.similarMovies == null || this.similarMovies.Count == 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Иконки
            this._imageUtilities.LoadImageAsync(this._movie.BackdropPath, BackgroundImage);
            this._imageUtilities.LoadImageAsync(this._movie.PosterPath, PosterImageBrash);

            // Текстовые данные
            this.IdMovie = this._movie.Id;
            this.TitleMovie = this._movie.Title;
            this.DescriptionMovie = this._movie.Overview;
            this.GenreMovie = this._movieRepository.GetGenreByMovie(this._movie);
        }

        private void ToHome_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new();

            this.Hide();
            mainWindow.Show();
        }

        /// <summary>
        /// Медленный скрол
        /// </summary>
        private void ListBox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            this._mouseUtilities.PreviewMouseWheel(sender, e);
        }

        private async void ShowTrailler_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if(button != null && button.Tag is uint ID)
            {
                VideoMovieModel videoInfo = await this._movieRepository.GetVideoMovie(ID);

                var trailer = videoInfo.Results.FirstOrDefault(v => v.Site == "YouTube" && v.Type == "Trailer"); //https://www.youtube.com/watch?v={videoId}
                if(trailer == null) return;

                Process.Start($"https://www.youtube.com/watch?v={trailer.Key}");
            }
        }
    }
}
