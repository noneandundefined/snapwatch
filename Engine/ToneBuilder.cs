using snapwatch.Engine.DataSet;
using snapwatch.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var tonesQuantity = new (string Name, ushort Value)[]
            {
                ("anticipation", tones.Anticipation),
                ("joy",          tones.Joy),
                ("trust",        tones.Trust),
                ("sadness",      tones.Sadness)
            };

            var (Name, Value) = tonesQuantity.OrderByDescending(t => t.Value).First();

            return Name;
        }

        private ToneModel AnalizeTone(List<string> tokens)
        {
            string[] anticipation = AnticipationEmotionLexicon();
            string[] joy = JoyEmotionLexicon();
            string[] trust = TrustEmotionLexicon();
            string[] sadness = SadnessEmotionLexicon();

            ushort countAnticipation = 0;
            foreach (var token in tokens)
            {
                if (anticipation.Contains(token))
                {
                    countAnticipation++;
                }
            }

            ToneModel toneModel = new()
            {
                Anticipation = (ushort)tokens.Count(token => anticipation.Contains(token)),
                Joy = (ushort)tokens.Count(token => joy.Contains(token)),
                Trust = (ushort)tokens.Count(token => trust.Contains(token)),
                Sadness = (ushort)tokens.Count(token => sadness.Contains(token)),
            };

            return toneModel;
        }
    }
}
