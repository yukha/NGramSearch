using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NGramSearch
{
    public class SorensenDiceCoefficientIndex<TKeyType> : NGramIndex<TKeyType>
    {
        public SorensenDiceCoefficientIndex() : base(TrigramSize, new SimpleNormalizer())
        {
        }

        public SorensenDiceCoefficientIndex(int nGramSize) : base(nGramSize, new SimpleNormalizer())
        {
        }

        public SorensenDiceCoefficientIndex(int nGramSize, INormalizer normalizer) : base(nGramSize, normalizer)
        {
        }

        public override double CalculatePhraseSimilarity(IndexedItem<TKeyType> indexedItem, double intersections, IList<GroupedNGram> searchNgrams)
        {
            return 2 * intersections / (indexedItem.NgramCount + searchNgrams.Sum(x => x.TotalPhraseNGramCount));
        }
    }
}
