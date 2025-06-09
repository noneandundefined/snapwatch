using snapwatch.Core.Utilities;
using snapwatch.UI.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace snapwatch.UI.View
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private bool _isLoading = false;

        private readonly MouseUtilities _mouseUtilities;

        public Home()
        {
            InitializeComponent();

            this._mouseUtilities = new MouseUtilities();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = e.OriginalSource as ScrollViewer;
            if (scrollViewer == null || this._isLoading)
            {
                return;
            }

            if (scrollViewer.ScrollableHeight == 0)
            {
                return;
            }

            if (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight * 0.9)
            {
                this._isLoading = true;

                if (DataContext is MovieCardVM vm)
                {
                    vm.LoadMoreMovies();
                }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    this._isLoading = false;
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        /// <summary>
        /// Медленный скрол
        /// </summary>
        private void ListBox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            this._mouseUtilities.PreviewMouseWheel(sender, e);
        }
    }
}
