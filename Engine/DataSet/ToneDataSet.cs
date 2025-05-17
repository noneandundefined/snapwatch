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

        private string[] ReadEmotionLexicon(string path)
        {
            List<string> output = [];

            foreach (var line in File.ReadLines(path))
            {
                var parts = line.Split('\t');
                output.Add(parts[0]);
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
