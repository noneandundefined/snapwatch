using System;
using System.Windows;
using System.Windows.Input;

namespace snapwatch.Core.Service
{
    public class WPFHelper
    {
        private readonly MainWindow _main;

        public WPFHelper(MainWindow main)
        {
            this._main = main;
        }

        public void LoaderVisibilityVisible() => this._main.Loader.Visibility = Visibility.Visible;

        public void LoaderVisibilityCollapsed() => this._main.Loader.Visibility = Visibility.Collapsed;
    }

    public class UIException
    {
        public void Error(string overview, string title)
        {
            MessageBox.Show(overview.ToString(), title.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Warning(string overview, string title)
        {
            MessageBox.Show(overview.ToString(), title.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
