using snapwatch.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace snapwatch.Engine
{
    public class TFIDFBuilder
    {
        private readonly NLPBuilder _nlpBuilder;
        private Dictionary<string, int> vocabIndex;

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

        public float[][] ComputeTFIDFMatrix(string[] overview, List<string> _vocabulary)
        {
            int nDocs = overview.Count;
            int nTerms = _vocabulary.Count;

            float[][] matrix = new float[nDocs][];
            for (int i = 0; i < nDocs; i++)
            {
                matrix[i] = new float[nTerms];
            }

            if (this.vocabIndex == null)
            {
                this.vocabIndex = _vocabulary.Select((token, index) => new { token, index }).ToDictionary(k => k.token, v => v.index);
            }
        }
    }
}
