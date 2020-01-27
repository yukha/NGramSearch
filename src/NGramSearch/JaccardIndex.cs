using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NGramSearch
{
    public class JaccardIndex<TKeyType> : NGramIndex<TKeyType>
    {
        public JaccardIndex() : base(TrigramSize, new SimpleNormalizer())
        {
        }

        public JaccardIndex(int nGramSize) : base(nGramSize, new SimpleNormalizer())
        {
        }

        public JaccardIndex(int nGramSize, INormalizer normalizer) : base(nGramSize, normalizer)
        {
        }

        public override double CalculatePhraseSimilarity(IndexedItem<TKeyType> indexedItem, double intersections, IList<GroupedNGram> searchNgrams)
        {
            return intersections / (indexedItem.NgramCount + searchNgrams.Sum(x => x.TotalPhraseNGramCount) - intersections);
        }
    }
}
