using snapwatch.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace snapwatch.UI.PopUps
{
    /// <summary>
    /// Логика взаимодействия для StoryLoadingPopUp.xaml
    /// </summary>
    public partial class StoryLoadingPopUp : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DispatcherTimer _timer;
        private double _progressStep;
        private int _totalDurationSecond = 54;
        private int _elapsedTicks = 0;

        public StoryLoadingPopUp()
        {
            InitializeComponent();
            DataContext = this;

            this._progressStep = 100.0 / this._totalDurationSecond;

            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromSeconds(1);
            this._timer.Tick += Timer_Tick;
            this._timer.Start();
        }

        private string _progressBarText = "";
        public string ProgressBarText
        {
            get => this._progressBarText;
            set
            {
                this._progressBarText = value + "%";
                OnPropertyChanged();
            }
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this._elapsedTicks >= this._totalDurationSecond)
            {
                this._timer.Stop();
                return;
            }

            ProgressBar.Value += this._progressStep;
            this.ProgressBarText = this._progressStep.ToString();
            this._elapsedTicks++;
        }
    }
}
