using Newtonsoft.Json;
using System;
using System.IO;

namespace snapwatch.Core.Core
{
    public class AppConfig
    {
        [JsonProperty("MOVIES_JSON_READ")]
        public string MOVIES_JSON_READ { get; }

        [JsonProperty("MOVIES_PIDX_READ")]
        public string MOVIES_PIDX_READ { get; }

        [JsonProperty("DATA_NRC_EMOTION_JOY")]
        public string DATA_NRC_EMOTION_JOY { get; }

        [JsonProperty("DATA_NRC_EMOTION_SADNESS")]
        public string DATA_NRC_EMOTION_SADNESS { get; }

        [JsonProperty("DATA_NRC_EMOTION_TRUST")]
        public string DATA_NRC_EMOTION_TRUST { get; }

        [JsonProperty("DATA_NRC_EMOTION_ANTICIPATION")]
        public string DATA_NRC_EMOTION_ANTICIPATION { get; }

        [JsonProperty("TRANSLATE_WWW_URL")]
        public string TRANSLATE_WWW_URL { get; }

        [JsonProperty("API_KEY_TMDB")]
        public string API_KEY_TMDB { get; }

        public AppConfig()
        {
            this.MOVIES_JSON_READ = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Engine", "Data", "movies.json");
            this.MOVIES_PIDX_READ = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Engine", "Data", "movies.pidx");
            this.DATA_NRC_EMOTION_JOY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Engine", "Data", "NRC-Emotion-Lexicon", "joy-NRC-Emotion-Lexicon.txt");
            this.DATA_NRC_EMOTION_SADNESS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Engine", "Data", "NRC-Emotion-Lexicon", "sadness-NRC-Emotion-Lexicon.txt");
            this.DATA_NRC_EMOTION_TRUST = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Engine", "Data", "NRC-Emotion-Lexicon", "trust-NRC-Emotion-Lexicon.txt");
            this.DATA_NRC_EMOTION_ANTICIPATION = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Engine", "Data", "NRC-Emotion-Lexicon", "anticipation-NRC-Emotion-Lexicon.txt");
            this.TRANSLATE_WWW_URL = "https://ftapi.pythonanywhere.com/translate";
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
