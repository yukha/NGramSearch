using System.Collections.Generic;
using System.Linq;

namespace NGramSearch
{
    internal class IndexedItem<TKeyType>
    {
        private int _ngramCount = -1;
        public TKeyType Id { get; set; }
        public string NormalizedValue { get; set; }
        public bool Deleted { get; set; } // false is default value

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

    }
}
