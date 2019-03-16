using System;
using System.Collections.Generic;
using System.Text;

namespace NGramSearch
{
    internal class PivotIndexItem
    {
        public PivotIndexItem()
        {
            TotalCount = 0;
            IndexedItemReferences = new List<IndexedNgramProperty>();
        }
        public int TotalCount { get; set; }

        public List<IndexedNgramProperty> IndexedItemReferences {get;set;}
    }
}
