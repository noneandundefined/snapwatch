using System;
using System.IO;

namespace snapwatch.Core.Core
{
    public class AppConfig
    {
        /// <summary>
        /// путь к файлу json с фильмами
        /// </summary>
        [Newtonsoft.Json.JsonProperty("MOVIES_JSON_READ")]
        public string MOVIES_JSON_READ { get; }

        /// <summary>
        /// путь к файлу .pidx к индексации страниц у фильмов
        /// </summary>
        [Newtonsoft.Json.JsonProperty("MOVIES_PIDX_READ")]
        public string MOVIES_PIDX_READ { get; }

        /// <summary>
        /// путь к файлу .txt к JOY эмоциональной тональности
        /// </summary>
        [Newtonsoft.Json.JsonProperty("DATA_NRC_EMOTION_JOY")]
        public string DATA_NRC_EMOTION_JOY { get; }

        /// <summary>
        /// путь к файлу .txt к SADNESS эмоциональной тональности
        /// </summary>
        [Newtonsoft.Json.JsonProperty("DATA_NRC_EMOTION_SADNESS")]
        public string DATA_NRC_EMOTION_SADNESS { get; }

        /// <summary>
        /// путь к файлу .txt к TRUST эмоциональной тональности
        /// </summary>
        [Newtonsoft.Json.JsonProperty("DATA_NRC_EMOTION_TRUST")]
        public string DATA_NRC_EMOTION_TRUST { get; }

        /// <summary>
        /// путь к файлу .txt к ANTICIPATION эмоциональной тональности
        /// </summary>
        [Newtonsoft.Json.JsonProperty("DATA_NRC_EMOTION_ANTICIPATION")]
        public string DATA_NRC_EMOTION_ANTICIPATION { get; }

        /// <summary>
        /// путь к серверу для LSA/SVD алгоритмам
        /// </summary>
        [Newtonsoft.Json.JsonProperty("SERVER_API_ADDRESS")]
        public string SERVER_API_ADDRESS { get; }

        /// <summary>
        /// путь к серверу для перевода текста
        /// </summary>
        [Newtonsoft.Json.JsonProperty("TRANSLATE_WWW_URL")]
        public string TRANSLATE_WWW_URL { get; }

        /// <summary>
        /// API KEY для the movie database(tmdb)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("API_KEY_TMDB")]
        public string API_KEY_TMDB { get; }

        public AppConfig()
        {
            this.MOVIES_JSON_READ = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "snapwatch.Engine", "Data", "movies.json");
            this.MOVIES_PIDX_READ = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "snapwatch.Engine", "Data", "movies.pidx");
            this.DATA_NRC_EMOTION_JOY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "snapwatch.Engine", "Data", "NRC-Emotion-Lexicon", "joy-NRC-Emotion-Lexicon.txt");
            this.DATA_NRC_EMOTION_SADNESS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "snapwatch.Engine", "Data", "NRC-Emotion-Lexicon", "sadness-NRC-Emotion-Lexicon.txt");
            this.DATA_NRC_EMOTION_TRUST = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "snapwatch.Engine", "Data", "NRC-Emotion-Lexicon", "trust-NRC-Emotion-Lexicon.txt");
            this.DATA_NRC_EMOTION_ANTICIPATION = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "snapwatch.Engine", "Data", "NRC-Emotion-Lexicon", "anticipation-NRC-Emotion-Lexicon.txt");
            this.SERVER_API_ADDRESS = "http://localhost:8011/microservice/movie-service/movie/f/t/gost";
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
