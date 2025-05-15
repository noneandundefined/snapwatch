using snapwatch.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace snapwatch.ViewModel
{
    public class MovieCardVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            this._action = new UIActions(Application.Current.MainWindow as MainWindow);

            Application.Current.Dispatcher.Invoke(() =>
            {
                this._action.LoaderVisibilityVisible();
            });

            Task.Run(async () =>
            {
                var movies = await this._recommendationService.Get();
                Movies = new ObservableCollection<MovieModel>();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (movies != null)
                    {
                        this._action.LoaderVisibilityCollapsed();
                        foreach (var movie in movies)
                        {
                            Movies.Add(movie);
                        }
                    }
                });
            });
        }

    }
}
