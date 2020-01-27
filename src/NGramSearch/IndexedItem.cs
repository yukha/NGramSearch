using System.Collections.Generic;
using System.Linq;

namespace NGramSearch
{
    public class IndexedItem<TKeyType>
    {
        private int _ngramCount = -1;
        public TKeyType Id { get; set; }
        public string NormalizedValue { get; set; }
        public IList<GroupedNGram> NGrams { get; set; }

        public int NgramCount
        {
            get
            {
                if (_ngramCount < 0)
                {
                    _ngramCount = NGrams.Sum(x => x.TotalPhraseNGramCount);
                }
                return _ngramCount;
            }
        }

    }
}
