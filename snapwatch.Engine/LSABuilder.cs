using snapwatch.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace snapwatch.Engine
{
    public class LSABuilder
    {
        private readonly NLPBuilder _nlpBuilder;

        private List<string> _vocabulary;

        public LSABuilder(List<MoviesModel> movies)
        {
            this._nlpBuilder = new NLPBuilder();
        }

        public void Analyze(List<string> documents, ushort top = 5)
        {
            this.AddVocabulary(documents);

            uint nDocs = (uint)documents.Count;
            uint nTerms = (uint)this._vocabulary.Count;

            var matrix = MathNet.Matrix
        }

        private void AddVocabulary(List<string> documents)
        {
            if (this._vocabulary == null)
            {
                this._vocabulary = documents.AsParallel().SelectMany(document => this._nlpBuilder.Preprocess(document)).Distinct().OrderBy(word => word).ToList();
            }
        }
    }
}
