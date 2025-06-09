using snapwatch.Core.Core;
using snapwatch.Core.Interface;
using snapwatch.Core.Service;
using snapwatch.Core.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace snapwatch.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для MovieCard.xaml
    /// </summary>
    public partial class MovieCard : UserControl
    {
        /// <summary>
        /// Классы и методы
        /// </summary>
        private readonly Config _config;
        private readonly HttpConfig _httpConfig;
        private readonly ImageUtilities _imageUtilities;
        private readonly ICacheRepository _cacheRepository = App._cacheRepository;

        private bool _imageLoaded = false;

        public MovieCard()
        {
            InitializeComponent();

            this._config = new Config();
            this._httpConfig = new HttpConfig();
            this._imageUtilities = new ImageUtilities();

            this.Loaded += MovieCard_Loaded;
            this.IsVisibleChanged += MovieCard_IsVisibleChanged;
        }

        private void MovieCard_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsVisible && !this._imageLoaded && !string.IsNullOrEmpty(PosterPath))
            {
                this._imageLoaded = true;
                this._imageUtilities.LoadImageAsync(PosterPath, MovieBrash);
            }
        }

        private void MovieCard_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && !this._imageLoaded && !string.IsNullOrEmpty(PosterPath))
            {
                this._imageLoaded = true;
                this._imageUtilities.LoadImageAsync(PosterPath, MovieBrash);
            }
        }

        private void CardLoadImageAsync(string path)
        {
            this._imageUtilities.LoadImageAsync(path, MovieBrash);
        }

        private static void OnPosterPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = (MovieCard)d;
            if (card.IsVisible && !card._imageLoaded && e.NewValue is string newPath && !string.IsNullOrEmpty(newPath))
            {
                card._imageLoaded = true;
                card.CardLoadImageAsync(newPath);
            }
        }

        private void ButtonDetailsMovie_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var gridButton = sender as Grid;

            if (gridButton != null && gridButton.Tag is uint ID)
            {
                DetailsWindow detailsWindow = new DetailsWindow(ID);
                Window mainWindow = Window.GetWindow(this); 

                detailsWindow.Show();
                mainWindow.Hide();
            }
        }

        /// <summary>
        /// Title Dependency
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MovieCard), new PropertyMetadata(string.Empty));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// PosterPath Dependency
        /// </summary>
        public static readonly DependencyProperty PosterPathProperty = DependencyProperty.Register("PosterPath", typeof(string), typeof(MovieCard), new PropertyMetadata(null, OnPosterPathChanged));
        public string PosterPath
        {
            get { return (string)GetValue(PosterPathProperty); }
            set { SetValue(PosterPathProperty, value); }
        }

        public ScaleTransform ScaleTransform => scaleTransform;
    }
}
