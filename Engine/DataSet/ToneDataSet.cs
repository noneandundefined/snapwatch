using snapwatch.Core.Core;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;

namespace snapwatch.Engine.DataSet
{
    public class ToneDataSet
    {
        private readonly Config _config;

        public ToneDataSet()
        {
            this._config = new Config();
        }

        protected string[] anticipationEmotionLexicon()
        {
            List<string> output = new List<string>();

            foreach (var line in File.ReadLines(this._config.ReturnConfig().DATA_NRC_EMOTION_ANTICIPATION))
            {
                var parts = line.Split('\t');
                output.Add(parts[0]);
            }

            return output.ToArray();
        }

        protected string[] joyfulKeywords()
        {

        }

        protected string[] sadKeywords()
        {

        }

        protected string[] calmKeywords()
        {

        }
    }
}
