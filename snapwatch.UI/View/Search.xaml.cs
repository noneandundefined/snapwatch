using snapwatch.Core.Interface;
using snapwatch.Core.Models;
using snapwatch.Core.Utilities;
using snapwatch.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;

namespace snapwatch.UI.View
{
    /// <summary>
    /// Логика взаимодействия для Search.xaml
    /// </summary>
    public partial class Search : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly MouseUtilities _mouseUtilities;
        private readonly LSABuilder _lsaBuilder = App._lsaBuilder;
        private readonly IMovieRepository _movieRepository = App._movieRepository;
        private readonly NLPBuilder _nlpBuilder;

        private DispatcherTimer timer;

        public Search()
        {
            InitializeComponent();
            DataContext = this;

            this._mouseUtilities = new MouseUtilities();
            this._nlpBuilder = new NLPBuilder();

            this.timer = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            this.timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Получение и вывод в UI похожии фильмы
        /// </summary>
        private HashSet<MovieModel> findMovies = [];
        public HashSet<MovieModel> FindMovies
        {
            get => this.findMovies;
            set
            {
                this.findMovies = value;
                OnPropertyChanged(nameof(FindMovies));
                OnPropertyChanged(nameof(HasNoMovies));
            }
        }

        /// <summary>
        /// Проверка на пустой ли список с фильмами
        /// </summary>
        public bool HasNoMovies => this.findMovies == null || this.findMovies.Count == 0;

        /// <summary>
        /// Медленный скрол
        /// </summary>
        private void ListBox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            this._mouseUtilities.PreviewMouseWheel(sender, e);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.timer.IsEnabled) this.timer.Stop();

            if(string.IsNullOrEmpty(SearchTextBox.Text))
            {
                PlaceholderSearch.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PlaceholderSearch.Visibility = System.Windows.Visibility.Collapsed;
                this.timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer.Stop();

            this.SearchMovies();
        }

        private void SearchMovies()
        {
            if(string.IsNullOrEmpty(SearchTextBox.Text))
            {
                this.FindMovies = [];
                return;
            }

            List<MoviesModel> movies = this._movieRepository.GetDataFileMovie();

            List<MovieModel> fillteredMovies = movies.AsParallel().SelectMany(g => g.Results).ToList();

            this.FindMovies = fillteredMovies.Where(movie => movie.Title.Contains(SearchTextBox.Text)).ToHashSet();

            //var similars = this._lsaBuilder.TFIDF_Cosine_Title(fillteredMovies, SearchTextBox.Text);

            //this.FindMovies = similars.Select(group => group.movies).Where(m => m.Title.Contains(SearchTextBox.Text)).Take(5).ToHashSet();
            //this.FindMovies = similars.Select(group => group.movies).ToHashSet();
        }
    }
}
