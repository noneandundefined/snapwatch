using snapwatch.UserInterface.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace snapwatch.UserInterface.View
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
