using snapwatch.Core.Interface;
using snapwatch.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace snapwatch.UI.View
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

        private readonly IMovieRepository _movieRepository = App._movieRepository;

        private string _selectTone = "";

        private HashSet<MovieModel> _movies = [];
        public HashSet<MovieModel> Movies
        {
            get => this._movies;
            set
            {
                this._movies = value;
                OnPropertyChanged();
            }
        }

        private HashSet<MovieModel> _moviesPreload = [];
        public HashSet<MovieModel> MoviesPreload
        {
            get => this._moviesPreload;
            set
            {
                this._moviesPreload = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => this._isLoading;
            set
            {
                if (this._isLoading != value)
                {
                    this._isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public Story()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void StoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderStory.Visibility = SearchTextBox.Text.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AnticipationToneButton_Click(object sender, RoutedEventArgs e) => this._selectTone = "anticipation";

        private void JoyToneButton_Click(object sender, RoutedEventArgs e) => this._selectTone = "joy";

        private void TrustToneButton_Click(object sender, RoutedEventArgs e) => this._selectTone = "trust";

        private void SadnessToneButton_Click(object sender, RoutedEventArgs e) => this._selectTone = "sadness";

        private async void Search_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.IsLoading = true;
                this.Movies = null;

                if (SearchTextBox.Text.Length > 0)
                {
                    this.MoviesPreload = await this._movieRepository.GetMoviesByText_Simple(SearchTextBox.Text);
                    StoryLoadingPopUp.Visibility = Visibility.Visible;
                    //this.Movies = await this._movieRepository.GetMoviesByText_HardAsync(SearchTextBox.Text);
                }
                else
                {
                    this.Movies = await this._movieRepository.GetMoviesByToneAsync(this._selectTone);
                }
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        /// <summary>
        /// Медленный скрол
        /// </summary>
        private void ListBox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = FindScrollViewer(sender as DependencyObject);
            if(scrollViewer != null)
            {
                double scrollAmount = 40;
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - Math.Sign(e.Delta) * scrollAmount);
                e.Handled = true;
            }
        }

        private ScrollViewer FindScrollViewer(DependencyObject d)
        {
            if(d is ScrollViewer viewer) return viewer;

            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                var result = FindScrollViewer(child);
                if(result != null) return result;
            }

            return null;
        }
    }
}
