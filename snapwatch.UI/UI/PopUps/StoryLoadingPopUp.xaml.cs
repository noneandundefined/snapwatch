using snapwatch.Core.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace snapwatch.UI.PopUps
{
    /// <summary>
    /// Логика взаимодействия для StoryLoadingPopUp.xaml
    /// </summary>
    public partial class StoryLoadingPopUp : UserControl
    {
        public StoryLoadingPopUp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Movies Dependency
        /// </summary>
        public static readonly DependencyProperty MoviesProperty = DependencyProperty.Register("Movies", typeof(HashSet<MovieModel>), typeof(StoryLoadingPopUp), new PropertyMetadata(null));
        public HashSet<MovieModel> Movies
        {
            get { return (HashSet<MovieModel>)GetValue(MoviesProperty); }
            set { SetValue(MoviesProperty, value); }
        }
    }
}
