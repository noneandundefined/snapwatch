using System.Windows;
using System.Windows.Controls;
using snapwatch.Core.Core;
using System.Windows.Media;
using snapwatch.Core.Service;
using snapwatch.Core.Interface;
using snapwatch.Core.Utilities;

namespace snapwatch.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для MovieCardMini.xaml
    /// </summary>
    public partial class MovieCardMini : UserControl
    {
        /// <summary>
        /// Классы и методы
        /// </summary>
        private readonly Config _config;
        private readonly HttpConfig _httpConfig;
        private readonly ImageUtilities _imageUtilities;
        private readonly ICacheRepository _cacheRepository = App._cacheRepository;

        private bool _imageLoaded = false;

        public MovieCardMini()
        {
            InitializeComponent();

            this._config = new Config();
            this._httpConfig = new HttpConfig();
            this._imageUtilities = new ImageUtilities();
        }

        private void MovieCardMini_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsVisible && !this._imageLoaded && !string.IsNullOrEmpty(PosterPath))
            {
                this._imageLoaded = true;
                this._imageUtilities.LoadImageAsync(PosterPath, MovieBrash);
            }
        }

        private void MovieCardMini_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && !this._imageLoaded && !string.IsNullOrEmpty(PosterPath))
            {
                this._imageLoaded = true;
                this._imageUtilities.LoadImageAsync(PosterPath, MovieBrash);
            }
        }

        /// <summary>
        /// Загрузка постеров
        /// </summary>
        /// <param name="path">путь изображения</param>
        private void CardLoadImageAsync(string path)
        {
            this._imageUtilities.LoadImageAsync(path, MovieBrash);
        }

        private static void OnPosterPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = (MovieCardMini)d;
            if (card.IsVisible && !card._imageLoaded && e.NewValue is string newPath && !string.IsNullOrEmpty(newPath))
            {
                card._imageLoaded = true;
                card.CardLoadImageAsync(newPath);
            }
        }

        /// <summary>
        /// Title Dependency
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MovieCardMini), new PropertyMetadata(string.Empty));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// PosterPath Dependency
        /// </summary>
        public static readonly DependencyProperty PosterPathProperty = DependencyProperty.Register("PosterPath", typeof(string), typeof(MovieCardMini), new PropertyMetadata(null, OnPosterPathChanged));
        public string PosterPath
        {
            get { return (string)GetValue(PosterPathProperty); }
            set { SetValue(PosterPathProperty, value); }
        }

        public ScaleTransform ScaleTransform => scaleTransform;
    }
}
