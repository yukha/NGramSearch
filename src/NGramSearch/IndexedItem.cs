using System;
using System.Collections.Generic;
using System.Text;

namespace NGramSearch
{
    internal class IndexedItem<TKeyType>
    {
        public TKeyType Id { get; set; }
        public string NormalizedValue { get; set; }

        public IList<string> Ngrams { get; set; }
    }
}
