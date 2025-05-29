using DotNetEnv;
using snapwatch.Core.Interface;
using snapwatch.Core.Repository;
using snapwatch.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace snapwatch
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IMovieRepository _movieRepository;
        public static LSABuilder _lsaBuilder;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Env.Load();

            _movieRepository = new MovieRepository();
            _lsaBuilder = new LSABuilder();
        }
    }
}
