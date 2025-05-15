using Newtonsoft.Json;

namespace snapwatch.Internal.Core
{
    public class AppConfig
    {
        [JsonProperty("SERVER_URL")]
        public string MOVIES_JSON_READ { get; }

        [JsonProperty("SERVER_URL")]
        public string API_KEY_TMDB { get; }

        public AppConfig()
        {
            this.MOVIES_JSON_READ = "pack://application:,,,/Internal/Data/movies.json";
            this.API_KEY_TMDB = "ecfe8540ac63325e0c50686c0be8848d";
        }
    }

    public class Config
    {
        public AppConfig ReturnConfig()
        {
            return new AppConfig();
        }
    }
}
