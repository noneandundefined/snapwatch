using snapwatch.Core.Service;
using snapwatch.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace snapwatch.UserInterface.ViewModel
{
    public class PanelVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Название меню в панеле
        /// </summary>
        public ObservableCollection<PanelItemsModel> MenuItems { get; set; }

        /// <summary>
        /// Команды
        /// </summary>
        private ICommand _menuItemClickCommand;
        public ICommand MenuItemClickCommand
        {
            get
            {
                return _menuItemClickCommand ?? (_menuItemClickCommand = new RelayCommand(path =>
                {
                    Navigation _nav = new Navigation(Application.Current.MainWindow as MainWindow);
                    _nav.SetFrameContent(path as string);
                }));
            }
        }

        public PanelVM()
        {
            MenuItems = new ObservableCollection<PanelItemsModel>
            {
                new PanelItemsModel { Image="pack://application:,,,/UserInterface/Public/ico/layout-dashboard.png", PathView="pack://application:,,,/UserInterface/View/Home.xaml", Title="Home" },
                new PanelItemsModel { Image="pack://application:,,,/UserInterface/Public/ico/text.png", PathView="pack://application:,,,/UserInterface/View/Story.xaml", Title="Story" },
            };
        }
    }
}
