using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NGramSearch
{
    public class NGramIndex<TKeyType>
    {
        private delegate double CalculatePhraseSimilarity(IndexedItem<TKeyType> indexedItem, double intersections, IList<GroupedNgram> searchNgrams);
        private delegate double CalculateNgramSimilarity(GroupedNgram searchedNgram, IndexedNgramProperty indexedNgram);

        private readonly int NCount;
        private readonly INormalizer Normalizer;

        private readonly object _itemListLock = new object();
        private readonly object _pivotIndexLock = new object();

        private readonly List<IndexedItem<TKeyType>> _itemList = new List<IndexedItem<TKeyType>>();

        private readonly IDictionary<string, PivotIndexItem> _pivotIndex
            = new Dictionary<string, PivotIndexItem>();


        public NGramIndex() : this(3, new SimpleNormalizer())
        {
        }

        public NGramIndex(int nCount) : this(nCount, new SimpleNormalizer())
        {
        }

        public NGramIndex(INormalizer normalizer) : this(3, normalizer)
        {
        }

        public NGramIndex(int nCount, INormalizer normalizer)
        {
            NCount = nCount;
            Normalizer = normalizer;
        }


        public void Add(TKeyType primaryKey, string value)
        {
            IndexedItem<TKeyType> cachedItem = new IndexedItem<TKeyType> { Id = primaryKey, NormalizedValue = Normalizer.Normalize(value) };
            cachedItem.Ngrams = CreateNgrams(cachedItem.NormalizedValue, NCount);

            int newItemListIndex;
            lock (_itemListLock)
            {
                _itemList.Add(cachedItem);
                newItemListIndex = _itemList.Count - 1;
            }

            foreach (GroupedNgram groupedSyllable in cachedItem.Ngrams)
            {
                PivotIndexItem pivotIndexItem;
                lock (_pivotIndexLock)
                {
                    if (!_pivotIndex.ContainsKey(groupedSyllable.Ngram))
                    {
                        _pivotIndex[groupedSyllable.Ngram] = new PivotIndexItem();
                    }

                    // add position to the list of positions
                    pivotIndexItem = _pivotIndex[groupedSyllable.Ngram];
                }

                pivotIndexItem.IndexedItemReferences.Add(
                    new IndexedNgramProperty(newItemListIndex, groupedSyllable.NgramCount));

                pivotIndexItem.TotalCount += groupedSyllable.NgramCount;
            }
        }

        public void Remove(TKeyType id)
        {
            IndexedItem<TKeyType> item = null;
            int wordIndex;
            lock (_itemListLock)
            {
                int len = _itemList.Count();

                for (wordIndex = 0; wordIndex < len; ++wordIndex)
                {
                    if (!_itemList[wordIndex].Deleted && EqualityComparer<TKeyType>.Default.Equals(_itemList[wordIndex].Id, id))
                    {
                        item = _itemList[wordIndex];
                        item.Deleted = true;
                        break;
                    }
                }
            }
            if (item == null)
            {
                return;
            }

            // remove from _pivotIndex
            lock (_pivotIndexLock)
            {
                IList<GroupedNgram> ngrams = CreateNgrams(item.NormalizedValue, NCount);
                foreach (GroupedNgram ngram in ngrams)
                {
                    PivotIndexItem pivotItem = _pivotIndex[ngram.Ngram];
                    // pivotItem.TotalCount -= ngram.NgramCount;
                    int pLen = pivotItem.IndexedItemReferences.Count();
                    for (int i = 0; i < pLen; ++i)
                    {
                        IndexedNgramProperty prop = pivotItem.IndexedItemReferences[i];
                        if (prop.WordIndex == wordIndex)
                        {
                            pivotItem.TotalCount -= ngram.NgramCount;
                            pivotItem.IndexedItemReferences.RemoveAt(i);
                            break;
                        }
                    }

                    if (pivotItem.TotalCount == 0)
                    {
                        _pivotIndex.Remove(ngram.Ngram);
                    }
                }
            }
        }

        /// <summary>
        /// Number of intersection
        /// </summary>
        /// <param name="searchedPhrase">Search phrase</param>
        /// <param name="reducePriorityOfNoisyNgrams">Calculate the weight of ngrams in the index and reduce the priority of noisy ngrams.</param>
        /// <returns></returns>
        public IEnumerable<ResultItem<TKeyType>> SearchWithIntersectionCount(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            if (reducePriorityOfNoisyNgrams)
            {
                return Search(searchedPhrase,
                             (searchedNgram, indexedNgram) => (double)Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount)
                                                                / _pivotIndex[searchedNgram.Ngram].TotalCount,
                             (indexedItem, intersections, searchNgrams) => intersections);
            }

            return Search(searchedPhrase,
                         (searchedNgram, indexedNgram) => Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount),
                         (indexedItem, intersections, searchNgrams) => intersections);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Simple_matching_coefficient
        /// intersection / (indexed_item_length + searchedPhrase_length)
        /// </summary>
        /// <param name="searchedPhrase">Search phrase</param>
        /// <param name="reducePriorityOfNoisyNgrams">Calculate the weight of ngrams in the index and reduce the priority of noisy ngrams.</param>
        /// <returns></returns>
        public IEnumerable<ResultItem<TKeyType>> SearchWithSimpleMatchingCoefficient(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            if (reducePriorityOfNoisyNgrams)
            {
                return Search(searchedPhrase,

                             (searchedNgram, indexedNgram) =>
                                (double)Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount)
                                 / _pivotIndex[searchedNgram.Ngram].TotalCount,

                             (indexedItem, intersections, searchNgrams) =>
                                intersections / (indexedItem.GetReducedPriorityNoisyNgramCount(_pivotIndex)
                                                 + searchNgrams.Sum(x => ((double)x.NgramCount)
                                                                          / (_pivotIndex.ContainsKey(x.Ngram) ? _pivotIndex[x.Ngram].TotalCount : 1))));
            }

            return Search(searchedPhrase,
                         (searchedNgram, indexedNgram) => Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount),
                         (indexedItem, intersections, searchNgrams) =>
                            intersections / (indexedItem.NgramCount + searchNgrams.Sum(x => x.NgramCount)));
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/S%C3%B8rensen%E2%80%93Dice_coefficient
        /// 2 * (intersection) / (indexed_item_length + searchedPhrase_length)
        /// </summary>
        /// <param name="searchedPhrase">Search phrase</param>
        /// <param name="reducePriorityOfNoisyNgrams">Calculate the weight of ngrams in the index and reduce the priority of noisy ngrams.</param>
        /// <returns></returns>
        public IEnumerable<ResultItem<TKeyType>> SearchWithSorensenDiceCoefficient(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            if (reducePriorityOfNoisyNgrams)
            {
                return Search(searchedPhrase,

                             (searchedNgram, indexedNgram) =>
                                (double)Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount)
                                 / _pivotIndex[searchedNgram.Ngram].TotalCount,

                             (indexedItem, intersections, searchNgrams) =>
                                2 * intersections / (indexedItem.GetReducedPriorityNoisyNgramCount(_pivotIndex)
                                                 + searchNgrams.Sum(x => ((double)x.NgramCount)
                                                                          / (_pivotIndex.ContainsKey(x.Ngram) ? _pivotIndex[x.Ngram].TotalCount : 1))));
            }
            return Search(searchedPhrase,
                         (searchedNgram, indexedNgram) => Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount),
                         (indexedItem, intersections, searchNgrams) =>
                            2 * intersections / (indexedItem.NgramCount + searchNgrams.Sum(x => x.NgramCount)));
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Jaccard_index
        /// intersection / (indexed_item_length + searchedPhrase_length - intersection)
        /// </summary>
        /// <param name="searchedPhrase">Search phrase</param>
        /// <param name="reducePriorityOfNoisyNgrams">Calculate the weight of ngrams in the index and reduce the priority of noisy ngrams.</param>
        /// <returns></returns>
        public IEnumerable<ResultItem<TKeyType>> SearchWithJaccardIndex(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            if (reducePriorityOfNoisyNgrams)
            {
                return Search(
                    searchedPhrase,

                    (searchedNgram, indexedNgram) =>
                        (double)Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount)
                        / _pivotIndex[searchedNgram.Ngram].TotalCount,

                    (indexedItem, intersections, searchNgrams) =>
                                intersections / (indexedItem.GetReducedPriorityNoisyNgramCount(_pivotIndex)
                                                 + searchNgrams.Sum(x => ((double)x.NgramCount)
                                                                          / (_pivotIndex.ContainsKey(x.Ngram) ? _pivotIndex[x.Ngram].TotalCount : 1))
                                                 - intersections));
            }
            return Search(searchedPhrase,
                         (searchedNgram, indexedNgram) => Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount),
                         (indexedItem, intersections, searchNgrams) =>
                            intersections / (indexedItem.NgramCount + searchNgrams.Sum(x => x.NgramCount) - intersections));
        }



        private IEnumerable<ResultItem<TKeyType>> Search(string searchedPhrase,
                                                        CalculateNgramSimilarity calculateNgramSimilarity,
                                                        CalculatePhraseSimilarity calculatePhraseSimilarity)
        {
            List<ResultItem<TKeyType>> result = new List<ResultItem<TKeyType>>();

            string normalizedValue = Normalizer.Normalize(searchedPhrase);

            IList<GroupedNgram> searchNgrams = CreateNgrams(normalizedValue, NCount);

            Dictionary<int, double> intersections = new Dictionary<int, double>(); // index item primary key, number of intersections

            foreach (GroupedNgram groupedNgram in searchNgrams)
            {
                if (_pivotIndex.ContainsKey(groupedNgram.Ngram))
                {
                    foreach (IndexedNgramProperty indexedNgramProperty in _pivotIndex[groupedNgram.Ngram].IndexedItemReferences)
                    {
                        if (!intersections.ContainsKey(indexedNgramProperty.WordIndex))
                        {
                            intersections[indexedNgramProperty.WordIndex]
                                = calculateNgramSimilarity(groupedNgram, indexedNgramProperty);
                        }
                        else
                        {
                            intersections[indexedNgramProperty.WordIndex]
                               += calculateNgramSimilarity(groupedNgram, indexedNgramProperty);
                        }
                    }
                }
            }

            // we don't remove from _itemList -> don't neet to lock: lock (_itemListLock) 
            result.AddRange(
                intersections.Keys.Select(
                    itemIndex =>
                        new ResultItem<TKeyType>
                        {
                            Id = _itemList[itemIndex].Id,
                            Similarity = calculatePhraseSimilarity(_itemList[itemIndex], intersections[itemIndex], searchNgrams)
                        }));


            return result.OrderByDescending(s => s.Similarity);
        }

        /// <summary>
        /// used only for tests
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<GroupedNgram> GetAllNgrams()
        {
            return _pivotIndex.Select(kvPair => new GroupedNgram { Ngram = kvPair.Key, NgramCount = kvPair.Value.TotalCount });
        }


        private static IList<GroupedNgram> CreateNgrams(string str, int n)
        {
            List<string> list = new List<string>();
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

            return list.GroupBy(ngram => ngram).Select(g => new GroupedNgram
            {
                Ngram = g.Key,
                NgramCount = g.Count()
            }).ToList();
        }

    }
}
