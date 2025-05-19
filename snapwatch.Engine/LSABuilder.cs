using snapwatch.Core.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace snapwatch.Engine
{
    public class LSABuilder
    {
        private readonly NLPBuilder _nlpBuilder;
        private readonly TFIDFBuilder _tfidfBuilder;

        private List<string> _vocabulary;

        public LSABuilder()
        {
            this._nlpBuilder = new NLPBuilder();
            this._tfidfBuilder = new TFIDFBuilder();
        }

        //public void Analyze(List<string> documents, ushort top = 5)
        //{
        //    this.AddVocabulary(documents);

        //    int nDocs = documents.Count;
        //    int nTerms = this._vocabulary.Count;

        //    var matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.Dense(nDocs, nTerms);

        //    for (int iDoc = 0; iDoc < nDocs; iDoc++)
        //    {
        //        List<string> tokens = this._nlpBuilder.Preprocess(documents[iDoc]);

        //        foreach (string token in tokens.Distinct())
        //        {
        //            int index = this._vocabulary.IndexOf(token);
        //            if (index == -1) continue;

        //            float tf = this._tfidfBuilder.TF(token, tokens.ToArray());
        //            double idf = this._tfidfBuilder.IDF()
        //        }
        //    }
        //}

        public List<(MovieModel, double Similarity)> AnalyzeByMovie(List<MovieMode> documents, string text, ushort top = 5)
        {
            documents.Add(text);
            this.AddVocabulary(documents);

            int nDocs = documents.Count;
            int nTerms = this._vocabulary.Count;

            var matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.Dense(nDocs, nTerms);

            for (int i = 0; i < nDocs; i++)
            {
                List<string> tokens = this._nlpBuilder.Preprocess(documents[i]);

                foreach (string token in tokens.Distinct())
                {
                    int index = this._vocabulary.IndexOf(token);
                    if (index == -1) continue;

                    float tf = this._tfidfBuilder.TF(token, tokens.ToArray());
                    double idf = this._tfidfBuilder.IDF(token, documents);

                    matrix[i, index] = this._tfidfBuilder.TFIDF(tf, idf);
                }
            }

            var svd = matrix.Svd(computeVectors: true);
            var documentTopicMatrix = svd.U;

            var vUser = documentTopicMatrix.Row(nDocs - 1);

            var similarities = new List<(MovieModel, double Similarity)>();

            for (int i = 0; i < nDocs - 1; i++)
            {
                var vMovie = documentTopicMatrix.Row(i);
                double similarity = this.CosineSimilarity(vUser, vMovie);
                similarities.Add((documents[i], similarity));
            }

            return similarities.OrderByDescending(x => x.Similarity).Take(top).ToList();
        }

        private void AddVocabulary(List<string> documents)
        {
            if (this._vocabulary == null)
            {
                this._vocabulary = documents.AsParallel().SelectMany(document => this._nlpBuilder.Preprocess(document)).Distinct().OrderBy(word => word).ToList();
            }
        }

        private double CosineSimilarity(MathNet.Numerics.LinearAlgebra.Vector<double> a, MathNet.Numerics.LinearAlgebra.Vector<double> b)
        {
            return a.DotProduct(b) / (a.L2Norm() * b.L2Norm() + 1e-10);
        }
    }
}
