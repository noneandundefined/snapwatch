using snapwatch.UI.ViewModel;
using System;
using System.Windows.Controls;

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
            var scrollViewer = sender as ScrollViewer;
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
    }
}
