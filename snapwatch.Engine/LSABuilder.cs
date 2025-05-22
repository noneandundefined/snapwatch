using snapwatch.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snapwatch.Engine
{
    public class LSABuilder
    {
        private readonly NLPBuilder _nlpBuilder;
        private readonly TFIDFBuilder _tfidfBuilder;

        private List<string> _vocabulary;
        private Dictionary<string, int> _vocabularyIndexMap;
        private List<List<string>> _tokenizedDOCS;
        private Dictionary<string, double> _idfCache;

        private readonly ushort avgOverview = 39;

        public LSABuilder()
        {
            this._nlpBuilder = new NLPBuilder();
            this._tfidfBuilder = new TFIDFBuilder();
        }

        public List<(MovieModel, double Similarity)> AnalyzeByMovie(List<MovieModel> documents, string text, ushort top = 25)
        {
            var documentsTake = documents.Take(documents.Count() / 6).ToList();
            List<string> dOverview = documentsTake.Select(document => document.Overview ?? "").ToList();
            dOverview.Add(text);

            this.AddVocabulary(dOverview);
            this.CalcIDF();

            int nDocs = dOverview.Count;
            int nTerms = this._vocabulary.Count;

            var matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.Dense(nDocs, nTerms);

            var localMatrix = new (int row, int col, double value)[nDocs][];

            Parallel.For(0, nDocs, i =>
            {
                List<string> tokens = this._tokenizedDOCS[i];
                Dictionary<string, int> tokenCountsDict = tokens.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
                int tokenTotal = tokens.Count();

                var rowMatrix = new List<(int, int, double)>();

                foreach (string token in tokenCountsDict.Keys)
                {
                    if (!this._idfCache.TryGetValue(token, out var idf)) continue;
                    if (!this._vocabularyIndexMap.TryGetValue(token, out var index)) continue;

                    if (index < 0 || index >= matrix.ColumnCount) continue;

                    //float tf = (float)tokenCountsDict[token] / tokenTotal;
                    float tf = this._tfidfBuilder.TF(tokenCountsDict[token], tokenTotal);
                    double value = this._tfidfBuilder.TFIDF(tf, idf);

                    rowMatrix.Add((i, index, value));

                    if (i >= 0 && i < matrix.RowCount)
                    {
                        matrix[i, index] = this._tfidfBuilder.TFIDF(tf, idf);
                    }
                }

                localMatrix[i] = [..rowMatrix];
            });

            var svd = matrix.Svd(computeVectors: true);
            var documentTopicMatrix = svd.U;

            var vUser = documentTopicMatrix.Row(nDocs - 1);

            var similarities = new List<(MovieModel, double Similarity)>();

            Parallel.For(0, documentsTake.Count(), i =>
            {
                var vMovie = documentTopicMatrix.Row(i);

                double similarity = this.CosineSimilarity(vUser, vMovie);
                lock (similarities)
                {
                    similarities.Add((documentsTake[i], similarity));
                }
            });

            return similarities.OrderByDescending(x => x.Similarity).Take(top).ToList();
        }

        private void AddVocabulary(List<string> documents)
        {
            if (this._vocabulary == null)
            {
                this._tokenizedDOCS = documents.AsParallel().Select(doc => this._nlpBuilder.Preprocess(doc)).ToList();

                this._vocabulary = this._tokenizedDOCS.AsParallel().
                            SelectMany(doc => doc).
                            GroupBy(word => word).
                            OrderByDescending(g => g.Count()).
                            Take(this.avgOverview * documents.Count()).
                            Select(g => g.Key).
                            OrderBy(word => word).
                            ToList();

                this._vocabularyIndexMap = this._vocabulary.Select((word, index) => new { word, index }).ToDictionary(x => x.word, x => x.index);
            }
        }

        private void CalcIDF()
        {
            this._idfCache = [];
            int N = this._tokenizedDOCS.Count();

            foreach (string word in this._vocabulary)
            {
                int df = this._tokenizedDOCS.Count(doc => doc.Contains(word));
                this._idfCache[word] = this._tfidfBuilder.IDF(N, df);
            }
        }

        private double CosineSimilarity(MathNet.Numerics.LinearAlgebra.Vector<double> a, MathNet.Numerics.LinearAlgebra.Vector<double> b)
        {
            return a.DotProduct(b) / (a.L2Norm() * b.L2Norm() + 1e-10);
        }
    }
}
