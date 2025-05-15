using Newtonsoft.Json;

namespace snapwatch.Internal.Core
{
    public class AppConfig
    {
        [JsonProperty("SERVER_URL")]
        public string MOVIES_JSON_READ { get; }

        public AppConfig()
        {
            this.MOVIES_JSON_READ = "pack://application:,,,/Internal/Data/movies.json";
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
