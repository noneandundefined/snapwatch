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

        public float IDF(string word, List<string> documents)
        {
            ushort cWord = (ushort)documents.AsParallel().Count(dOverview =>
            {
                List<string> overview = this._nlpBuilder.Preprocess(dOverview);
                return overview.Contains(word);
            });

            return (float)Math.Log(documents.Count / (1 + cWord));
        }

        public float TFIDF(float tf, float idf)
        {
            return tf * idf;
        }

        public float[][] ComputeTFIDFMatrix(string[] overviews, List<string> _vocabulary)
        {
            int nDocs = overviews.Count();
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

            var idfCache = _vocabulary.ToDictionary(
                token => token,
                token => this.IDF(token, [..overviews])
            );

            Parallel.For(0, nDocs, i =>
            {
                List<string> tokens = _nlpBuilder.Preprocess(overviews[i]);
                Dictionary<string, float> tfCache = new();

                foreach (var token in tokens)
                {
                    if (!vocabIndex.TryGetValue(token, out int index)) continue;

                    if (!tfCache.TryGetValue(token, out float tf))
                    {
                        tf = this.TF(token, [.. tokens]);
                        tfCache[token] = tf;
                    }

                    if (!idfCache.TryGetValue(token, out float idf)) continue;

                    matrix[i][index] = this.TFIDF(tf, idf);
                }
            });

            return matrix;
        }
    }
}
