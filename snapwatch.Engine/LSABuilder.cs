using snapwatch.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snapwatch.Engine
{
    public class LSABuilder
    {
        /// <summary>
        /// Классы и методы
        /// </summary>
        private readonly NLPBuilder _nlpBuilder;
        private readonly TFIDFBuilder _tfidfBuilder;

        /// <summary>
        /// слова участвующие в поиске
        /// </summary>
        private HashSet<string> _vocabulary;

        /// <summary>
        /// подготовленные(NLP) слова
        /// </summary>
        private List<List<string>> _tokenizedDOCS;

        /// <summary>
        /// список TF-IDF векторов для всех документов
        /// </summary>
        private List<double[]> _tfidfVectors = [];

        /// <summary>
        /// словарь: слово -> IDF значение (частота документа)
        /// </summary>
        private Dictionary<string, double> _idfCache;

        /// <summary>
        /// словарь: слово -> количество документов где встречается это это слово
        /// </summary>
        private ConcurrentDictionary<string, uint> _docsTokensCache = [];

        /// <summary>
        /// max. количество слов для одного документа
        /// </summary>
        private readonly ushort _avgOverview = 39;

        public LSABuilder()
        {
            this._nlpBuilder = new NLPBuilder();
            this._tfidfBuilder = new TFIDFBuilder();
        }

        /// <summary>
        /// подготовка и счет базовых значений + кеширование
        /// </summary>
        /// <param name="overviews">описания фильмов</param>
        public void Fit(string[] overviews)
        {
            if (this._vocabulary == null)
            {
                this._idfCache = [];

                this._tokenizedDOCS = overviews.AsParallel().Select(doc => this._nlpBuilder.Preprocess(doc)).ToList();
                this._vocabulary = _tokenizedDOCS.AsParallel().SelectMany(token => token).Take(this._avgOverview * overviews.Length).ToHashSet();

                Parallel.ForEach(this._tokenizedDOCS, doc =>
                {
                    foreach (var term in doc.Distinct())
                    {
                        this._docsTokensCache.AddOrUpdate(term, 1, (key, oldValue) => oldValue + 1);
                    }
                });

                foreach (string term in this._vocabulary)
                {
                    uint N = this._docsTokensCache.ContainsKey(term) ? this._docsTokensCache[term] : 0;
                    this._idfCache[term] = this._tfidfBuilder.IDF(this._tokenizedDOCS.Count, (int)N);
                }

                this._tfidfVectors = overviews.AsParallel().Select(doc => this.Transform(doc)).ToList();
            }
        }

        /// <summary>
        /// счет TF-IDF документов и NLP подготовка
        /// </summary>
        /// <param name="doc">документ (описание фильма)</param>
        private double[] Transform(string doc)
        {
            List<string> tokens = this._nlpBuilder.Preprocess(doc);
            double[] tfidf = new double[this._vocabulary.Count];
            List<string> vocabList = [..this._vocabulary];

            var termFreq = tokens.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());

            for (int i = 0; i < vocabList.Count; i++)
            {
                string term = vocabList[i];
                int termCount = termFreq.ContainsKey(term) ? termFreq[term] : 0;

                float tf = this._tfidfBuilder.TF(termCount, tokens.Count);
                tfidf[i] = this._tfidfBuilder.TFIDF(tf, this._idfCache.ContainsKey(term) ? this._idfCache[term] : 0.0);
            }

            return tfidf;
        }

        /// <summary>
        /// основная фукнция нахождения похожих фильмов
        /// </summary>
        /// <param name="documents">фильмы</param>
        /// <param name="text">текст написанный пользователем</param>
        /// <param name="top">максимальный вывод фильмов</param>
        public List<(MovieModel movies, double similarity)> TFIDF_Cosine(List<MovieModel> documents, string text, ushort top = 50)
        {
            var documentsTake = documents.Take(documents.Count / 2).ToList();
            List<string> overviews = documentsTake.AsParallel().Select(document => document.Overview ?? "").ToList();

            this.Fit([..overviews]);

            double[] vectorInput = this.Transform(text);
            List<(MovieModel movies, double similarity)> similarity = [];

            return documentsTake.AsParallel().Select((doc, i) => (
                movies: doc,
                similarity: this.CosineSimilarity(vectorInput, this._tfidfVectors[i])
            )).OrderByDescending(sim => sim.similarity).Take(top).ToList();
        }

        /// <summary>
        /// косинусное сравнение векторов
        /// </summary>
        /// <param name="vector_a"></param>
        /// <param name="vector_b"></param>
        private double CosineSimilarity(double[] vector_a, double[] vector_b)
        {
            double dot = 0, normA = 0, normB = 0;

            for (int i = 0; i < vector_a.Length; i++)
            {
                dot += vector_a[i] * vector_b[i];
                normA += vector_a[i] * vector_a[i];
                normB += vector_b[i] * vector_b[i];
            }

            return normA == 0 || normB == 0 ? 0 : dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }
}
