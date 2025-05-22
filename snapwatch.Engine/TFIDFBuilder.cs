using snapwatch.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snapwatch.Engine
{
    public class TFIDFBuilder
    {
        private readonly NLPBuilder _nlpBuilder;

        public TFIDFBuilder()
        {
            this._nlpBuilder = new NLPBuilder();
        }

        public float TF(string word, string[] text)
        {
            ushort cWord = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (word == text[i])
                {
                    cWord++;
                }
            }

            return (float)cWord / text.Length;
        }

        public double IDF(string word, List<string> documents)
        {
            ushort cWord = (ushort)documents.AsParallel().Count(dOverview =>
            {
                List<string> overview = this._nlpBuilder.Preprocess(dOverview);
                return overview.Contains(word);
            });

            return Math.Log((double)documents.Count / (1 + cWord));
        }

        public double TFIDF(float tf, double idf)
        {
            return tf * idf;
        }
    }
}
