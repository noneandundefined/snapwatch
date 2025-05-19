using snapwatch.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace snapwatch.Engine
{
    public class TFIDFBuilder
    {
        private readonly NLPBuilder _nlpBuilder;

        private readonly List<MovieModel> _movies;

        public TFIDFBuilder(List<MoviesModel> movies)
        {
            this._nlpBuilder = new NLPBuilder();

            this._movies = movies.AsParallel().SelectMany(movie => movie.Results).ToList();
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

        public double IDF(string word)
        {
            ushort cWord = (ushort)this._movies.AsParallel().Count(movie =>
            {
                List<string> overview = this._nlpBuilder.Preprocess(movie.Overview);
                return overview.Contains(word);
            });

            return Math.Log((double)this._movies.Count / (1 + cWord));
        }

        public double TFIDF(float tf, double idf)
        {
            return tf * idf;
        }
    }
}
