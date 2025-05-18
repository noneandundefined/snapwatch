using snapwatch.Core.Interface;
using snapwatch.Core.Repository;
using System.Windows;
using System.Windows.Controls;

namespace snapwatch.UserInterface.View
{
    /// <summary>
    /// Логика взаимодействия для Story.xaml
    /// </summary>
    public partial class Story : UserControl
    {
        private readonly IMovieRepository movieRepository;

        private string _selectTone = "";

        public Story()
        {
            InitializeComponent();
            this.movieRepository = new MovieRepository();
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

            this.movieRepository.GetMoviesByTone("sadness");
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
