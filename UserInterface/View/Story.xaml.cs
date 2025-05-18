using snapwatch.Core.Interface;
using snapwatch.Core.Models;
using snapwatch.Core.Repository;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace snapwatch.UserInterface.View
{
    /// <summary>
    /// Логика взаимодействия для Story.xaml
    /// </summary>
    public partial class Story : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IMovieRepository _movieRepository;

        private string _selectTone = "";

        private List<MovieModel> _movies = [];
        public List<MovieModel> Movies
        {
            get => this._movies;
            set
            {
                this._movies = value;
                OnPropertyChanged();
            }
        }

        public Story()
        {
            InitializeComponent();
            DataContext = this;

            this._movieRepository = new MovieRepository();
        }

        private void StoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text.Length > 0)
            {
                PlaceholderStory.Visibility = Visibility.Collapsed;
            }
            else
            {
                PlaceholderStory.Visibility = Visibility.Visible;
            }
        }

        private void AnticipationToneButton_Click(object sender, RoutedEventArgs e)
        {
            this._selectTone = "anticipation";
        }

        private void JoyToneButton_Click(object sender, RoutedEventArgs e)
        {
            this._selectTone = "joy";
        }

        private void TrustToneButton_Click(object sender, RoutedEventArgs e)
        {
            this._selectTone = "trust";
        }

        private void SadnessToneButton_Click(object sender, RoutedEventArgs e)
        {
            this._selectTone = "sadness";
        }

        private async void Search_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Movies = null;

            this.Movies = await this._movieRepository.GetMoviesByToneAsync(this._selectTone);
        }
    }
}
