using snapwatch.Core.Core;
using System.Collections.Generic;
using System.IO;

namespace snapwatch.Engine.DataSet
{
    public class ToneDataSet
    {
        private readonly Config _config;

        public ToneDataSet()
        {
            this._config = new Config();
        }

        protected List<int> AnticipationGenresID { get; set; } = [27, 28, 9648, 10752, 53, 878, 37];
        protected List<int> JoyGenresID { get; set; } = [80, 16, 28, 35, 14, 12, 37];
        protected List<int> TrustGenresID { get; set; } = [36, 99, 10751, 10770, 10749, 37];
        protected List<int> SadnessGenresID { get; set; } = [18, 10749, 10402, 10751, 16, 37];

        private string[] ReadEmotionLexicon(string path)
        {
            List<string> output = [];

            foreach (var line in File.ReadLines(path))
            {
                var parts = line.Split('\t');

                if (parts[1] == "1")
                {
                    output.Add(parts[0]);
                }
            }

            return [.. output];
        }

        protected string[] AnticipationEmotionLexicon()
        {
            return this.ReadEmotionLexicon(this._config.ReturnConfig().DATA_NRC_EMOTION_ANTICIPATION);
        }

        protected string[] JoyEmotionLexicon()
        {
            return this.ReadEmotionLexicon(this._config.ReturnConfig().DATA_NRC_EMOTION_JOY);
        }

        protected string[] TrustEmotionLexicon()
        {
            return this.ReadEmotionLexicon(this._config.ReturnConfig().DATA_NRC_EMOTION_TRUST);
        }

        protected string[] SadnessEmotionLexicon()
        {
            return this.ReadEmotionLexicon(this._config.ReturnConfig().DATA_NRC_EMOTION_SADNESS);
        }
    }
}
