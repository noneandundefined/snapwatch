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

        public Home()
        {
            InitializeComponent();
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
            if (d is ScrollViewer viewer) return viewer;

            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                var result = FindScrollViewer(child);
                if (result != null) return result;
            }

            return null;
        }
    }
}
