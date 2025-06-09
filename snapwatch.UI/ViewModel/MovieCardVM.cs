using snapwatch.Core.Interface;
using snapwatch.Core.Repository;
using snapwatch.Core.Service;
using snapwatch.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace snapwatch.UI.ViewModel
{
    public class MovieCardVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly WPFHelper _wpfHelper;

        private readonly IMovieRepository _movieRepository = App._movieRepository;

        /// <summary>
        /// Хранение фильмов
        /// </summary>
        private ObservableCollection<MovieModel> _movies;
        public ObservableCollection<MovieModel> Movies
        {
            get { return this._movies; }
            set
            {
                this._movies = value;
                OnPropertyChanged(nameof(Movies));
            }
        }

        public MovieCardVM()
        {
            this._wpfHelper = new WPFHelper(Application.Current.MainWindow as MainWindow);

            Application.Current.Dispatcher.Invoke(() =>
            {
                this._wpfHelper.LoaderVisibilityVisible();
            });

            Task.Run(() =>
            {
                MoviesModel movies = this._movieRepository.GetMovies();
                Movies = [];

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this._wpfHelper.LoaderVisibilityCollapsed();

                    if (this.Movies == null)
                    {
                        this.Movies = [];
                    }
                    else
                    {
                        this.Movies.Clear();
                    }

                    if (movies != null)
                    {
                        foreach(var movie in movies.Results)
                        {
                            Movies.Add(movie);
                        }
                    }
                });
            });
        }

        public void LoadMoreMovies()
        {
            MoviesModel movies = this._movieRepository.GetMovies();

            if (movies != null)
            {
                foreach (var movie in movies.Results)
                {
                    Movies.Add(movie);
                }
            }
        }
    }
}
