using System.Windows;

namespace snapwatch
{
    /// <summary>
    /// Логика взаимодействия для DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        private uint _movieID;

        public DetailsWindow(uint ID)
        {
            InitializeComponent();

            this._movieID = ID;
        }
    }
}
