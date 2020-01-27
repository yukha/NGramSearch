using System.Collections.Generic;

namespace NGramSearch
{
    public class IntersectionCountIndex<TKeyType> : NGramIndex<TKeyType>
    {
        public IntersectionCountIndex() : base(TrigramSize, new SimpleNormalizer())
        {
        }

        public IntersectionCountIndex(int nGramSize) : base(nGramSize, new SimpleNormalizer())
        {
        }

        public IntersectionCountIndex(int nGramSize, INormalizer normalizer) : base(nGramSize, normalizer)
        {
            
        }

        public override double CalculatePhraseSimilarity(IndexedItem<TKeyType> indexedItem, double intersections, IList<GroupedNGram> searchNgrams) 
            => intersections;
    }
}
