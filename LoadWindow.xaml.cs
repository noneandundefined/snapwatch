using snapwatch.Core.Interface;
using snapwatch.Core.Models;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System;
using snapwatch.Engine;
using System.Threading.Tasks;
using snapwatch.Core.Utilities;
using snapwatch.Core.Service;

namespace snapwatch
{
    /// <summary>
    /// Логика взаимодействия для LoadWindow.xaml
    /// </summary>
    public partial class LoadWindow : Window
    {
        private readonly IMovieRepository _movieRepository = App._movieRepository;
        private readonly LSABuilder _lsaBuilder = App._lsaBuilder;

        private readonly UIException _uiException;

        public LoadWindow()
        {
            InitializeComponent();

            this._uiException = new UIException();

            this.Preparation();
        }

        private void Preparation()
        {
            try
            {
                // 1
                this._movieRepository.GetMovies();

                // 2
                List<MoviesModel> documents = this._movieRepository.GetDataFileMovie();
                List<MovieModel> filteredMovies = documents.AsParallel().
                                                    WithDegreeOfParallelism(Environment.ProcessorCount).
                                                    SelectMany(group => group.Results).ToList();

                var documentsTake = filteredMovies.Shuffle().Take(filteredMovies.Count / 2).ToList();
                List<string> overviews = documentsTake.AsParallel().Select(document => document.Overview ?? "").ToList();

                this._lsaBuilder.Fit([.. overviews]);

                this.NextWindow();
            }
            catch (Exception ex)
            {
                this._uiException.Error(ex.Message, "Ошибка приложения");
            }
        }

        private async void NextWindow()
        {
            await Task.Delay(3000);

            MainWindow mainWindow = new();
            this.Hide();

            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}
