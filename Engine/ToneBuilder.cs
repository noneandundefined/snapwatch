using snapwatch.Engine.DataSet;
using snapwatch.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snapwatch.Engine
{
    public class ToneBuilder : ToneDataSet
    {
        private readonly NLPBuilder _nlpBuilder;

        public ToneBuilder()
        {
            this._nlpBuilder = new NLPBuilder();
        }

        public string Tone(string text)
        {
            List<string> tokens = this._nlpBuilder.Preprocess(text);

            ToneModel tones = this.AnalizeTone(tokens);

            return "";
        }

        private ToneModel AnalizeTone(List<string> tokens)
        {
            ToneModel toneModel = new();

            ushort countAnticipation = tokens.Count(token => AnticipationEmotionLexicon.Contains(token));
        }
    }
}
