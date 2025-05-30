using DotNetEnv;
using snapwatch.Core.Interface;
using snapwatch.Core.Repository;
using snapwatch.Engine;
using System.Windows;

namespace snapwatch
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IMovieRepository _movieRepository;
        public static ICacheRepository _cacheRepository;
        public static LSABuilder _lsaBuilder;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Env.Load();

            _movieRepository = new MovieRepository();
            _cacheRepository = new CacheRepository();
            _lsaBuilder = new LSABuilder();
        }
    }
}
