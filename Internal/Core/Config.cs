using Newtonsoft.Json;
using System;
using System.IO;

namespace snapwatch.Internal.Core
{
    public class AppConfig
    {
        [JsonProperty("MOVIES_JSON_READ")]
        public string MOVIES_JSON_READ { get; }

        [JsonProperty("MOVIES_PIDX_READ")]
        public string MOVIES_PIDX_READ { get; }

        [JsonProperty("API_KEY_TMDB")]
        public string API_KEY_TMDB { get; }

        public AppConfig()
        {
            this.MOVIES_JSON_READ = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Internal", "Data", "movies.json");
            this.MOVIES_PIDX_READ = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Internal", "Data", "movies.pidx");
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
