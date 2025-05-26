using snapwatch.Core.Interface;
using snapwatch.Core.Repository;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

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
        private readonly IMovieRepository _movieRepository;

        private uint _movieID;

        public DetailsWindow(uint ID)
        {
            InitializeComponent();
            this._movieRepository = new MovieRepository();

            this._movieID = ID;
        }

        public string Title { get; set; }
    }
}
