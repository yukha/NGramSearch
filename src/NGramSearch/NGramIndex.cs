using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NGramSearch
{
    public abstract class NGramIndex<TKeyType>
    {
        protected const int TrigramSize = 3;

        public abstract double CalculatePhraseSimilarity(IndexedItem<TKeyType> indexedItem, double intersections, IList<GroupedNGram> searchNGrams);

        protected readonly int NGramSize;
        protected readonly INormalizer Normalizer;

        private readonly List<IndexedItem<TKeyType>> _itemList = new List<IndexedItem<TKeyType>>();

        private readonly IDictionary<string, PivotIndexItem> _pivotIndex
            = new Dictionary<string, PivotIndexItem>();


        protected NGramIndex(int nCount, INormalizer normalizer)
        {
            NGramSize = nCount;
            Normalizer = normalizer;
        }
        
        public void Add(TKeyType primaryKey, string value)
        {
            var cachedItem = CreateAndInitializeIndexedItem(primaryKey, value);

            var newItemListIndex = AddItemToIndex(cachedItem);

            CalculateIndexForEachNgram(cachedItem, newItemListIndex);
        }

        private IndexedItem<TKeyType> CreateAndInitializeIndexedItem(TKeyType primaryKey, string value)
        {
            var cachedItem = new IndexedItem<TKeyType>
            {
                Id = primaryKey,
                NormalizedValue = Normalizer.Normalize(value)
            };
            cachedItem.NGrams = CreateNGrams(cachedItem.NormalizedValue, NGramSize);
            return cachedItem;
        }

        private int AddItemToIndex(IndexedItem<TKeyType> cachedItem)
        {
            _itemList.Add(cachedItem);
            return _itemList.Count - 1;
        }

        private void CalculateIndexForEachNgram(IndexedItem<TKeyType> cachedItem, int newItemListIndex)
        {
            foreach (GroupedNGram groupedSyllable in cachedItem.NGrams)
            {
                var pivotIndexItem = CreatePivotIndexItemIfNeed(groupedSyllable);

                pivotIndexItem.IndexedItemReferences.Add(
                    new IndexedNgramProperty(newItemListIndex, groupedSyllable.TotalPhraseNGramCount));

                pivotIndexItem.TotalCount += groupedSyllable.TotalPhraseNGramCount;
            }
        }

        private PivotIndexItem CreatePivotIndexItemIfNeed(GroupedNGram groupedSyllable)
        {
            if (!_pivotIndex.ContainsKey(groupedSyllable.NGram))
            {
                _pivotIndex[groupedSyllable.NGram] = new PivotIndexItem();
            }

            return _pivotIndex[groupedSyllable.NGram];
        }
        
        public IEnumerable<ResultItem<TKeyType>> Search(string searchedPhrase)
        {
            var searchNGrams = CreateNGramsFromPhrase(searchedPhrase);

            var intersections = CalculateIntersectionsForSearchedNGrams(searchNGrams);

            return intersections.Keys
                .Select(
                    itemIndex =>
                        new ResultItem<TKeyType>
                        {
                            Id = _itemList[itemIndex].Id,
                            Similarity = CalculatePhraseSimilarity(_itemList[itemIndex], intersections[itemIndex], searchNGrams)
                        })
                .OrderByDescending(s => s.Similarity);
        }

        private Dictionary<int, double> CalculateIntersectionsForSearchedNGrams(IList<GroupedNGram> searchNGrams)
        {
            Dictionary<int, double>
                intersections = new Dictionary<int, double>();

            foreach (GroupedNGram groupedNgram in searchNGrams)
            {
                CalculateIntersectionForNGram(groupedNgram, intersections);
            }

            return intersections;
        }

        private void CalculateIntersectionForNGram(GroupedNGram groupedNgram, Dictionary<int, double> intersections)
        {
            if (_pivotIndex.ContainsKey(groupedNgram.NGram))
            {
                foreach (IndexedNgramProperty indexedNgramProperty in _pivotIndex[groupedNgram.NGram].IndexedItemReferences)
                {
                    if (!intersections.ContainsKey(indexedNgramProperty.WordIndex))
                    {
                        intersections[indexedNgramProperty.WordIndex]
                            = Math.Min(groupedNgram.TotalPhraseNGramCount, indexedNgramProperty.NgramCount);
                    }
                    else
                    {
                        intersections[indexedNgramProperty.WordIndex]
                            += Math.Min(groupedNgram.TotalPhraseNGramCount, indexedNgramProperty.NgramCount);
                    }
                }
            }
        }

        private IList<GroupedNGram> CreateNGramsFromPhrase(string searchedPhrase)
        {
            string normalizedValue = Normalizer.Normalize(searchedPhrase);

            IList<GroupedNGram> searchNGrams = CreateNGrams(normalizedValue, NGramSize)
                                               ?? throw new ArgumentNullException("searchedPhrase");
            return searchNGrams;
        }

        /// <summary>
        /// used only for tests
        /// </summary>
        public IEnumerable<GroupedNGram> GetAllNGrams()
        {
            return _pivotIndex.Select(kvPair => new GroupedNGram { NGram = kvPair.Key, TotalPhraseNGramCount = kvPair.Value.TotalCount });
        }


        private static IList<GroupedNGram> CreateNGrams(string str, int n)
        {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(str))
            {
                int len = str.Length;
                for (int i = -1; i < len - n + 2; ++i)
                {
                    StringBuilder sb = new StringBuilder();
                    int p = i;
                    while (p < 0)
                    {
                        sb.Append(' ');
                        p++;
                    }
                    while (p - i < n && p < len)
                    {
                        sb.Append(str[p]);
                        p++;
                    }
                    while (p - i < n)
                    {
                        sb.Append(' ');
                        p++;
                    }
                    list.Add(sb.ToString());
                }

            }

            return list.GroupBy(ngram => ngram).Select(g => new GroupedNGram
            {
                NGram = g.Key,
                TotalPhraseNGramCount = g.Count()
            }).ToList();
        }

    }
}
