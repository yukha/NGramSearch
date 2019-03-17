using System.Collections.Generic;
using System.Linq;

namespace NGramSearch
{
    internal class IndexedItem<TKeyType>
    {
        private int _ngramCount = -1;
        private double _reducedNgramCount = -1;
        public TKeyType Id { get; set; }
        public string NormalizedValue { get; set; }

        public IList<GroupedNgram> Ngrams { get; set; }

        public int NgramCount
        {
            get
            {
                if (_ngramCount < 0)
                {
                    _ngramCount = Ngrams.Sum(x => x.NgramCount);
                }
                return _ngramCount;
            }
        }

        public double GetReducedPriorityNoisyNgramCount(IDictionary<string, PivotIndexItem> pivotIndex)
        {
            if(_reducedNgramCount < 0)
            {
                _reducedNgramCount = Ngrams.Sum(x => ((double)x.NgramCount) / pivotIndex[x.Ngram].TotalCount);
            }
            return _reducedNgramCount;
        }
    }
}
