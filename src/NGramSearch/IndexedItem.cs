using System.Collections.Generic;

namespace NGramSearch
{
    internal class IndexedItem<TKeyType>
    {
        public TKeyType Id { get; set; }
        public string NormalizedValue { get; set; }

        public IList<GroupedNgram> Ngrams { get; set; }
    }
}
