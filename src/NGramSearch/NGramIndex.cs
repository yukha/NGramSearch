using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NGramSearch
{
    public class NGramIndex<TKeyType>
    {
        private delegate double CalculatePhraseSimilarity(IndexedItem<TKeyType> indexedItem, double intersections, IList<GroupedNgram> searchNgrams);
        private delegate double CalculateNgramSimilarity(GroupedNgram searchedNgram, IndexedNgramProperty indexedNgram);

        private readonly int NCount;

        /// <summary>
        /// list of cached items
        /// </summary>
        private readonly List<IndexedItem<TKeyType>> _indexedItems = new List<IndexedItem<TKeyType>>();

        private readonly IDictionary<string, PivotIndexItem> _pivotIndex
            = new Dictionary<string, PivotIndexItem>();

        private readonly List<TKeyType> _ignoredItems = new List<TKeyType>();

        public NGramIndex() : this(3)
        {
        }

        public NGramIndex(int nCount)
        {
            NCount = nCount;
        }


        public void Add(TKeyType primaryKey, string normalizedValue)
        {

            var cachedItem = new IndexedItem<TKeyType> { Id = primaryKey, NormalizedValue = normalizedValue };
            cachedItem.Ngrams = CreateNgrams(cachedItem.NormalizedValue, NCount);

            _indexedItems.Add(cachedItem);


            foreach (GroupedNgram groupedSyllable in cachedItem.Ngrams)
            {
                if (!_pivotIndex.ContainsKey(groupedSyllable.Ngram))
                {
                    _pivotIndex[groupedSyllable.Ngram] = new PivotIndexItem();
                }

                // add position to the list of positions
                var pivotIndexItem = _pivotIndex[groupedSyllable.Ngram];

                pivotIndexItem.IndexedItemReferences.Add(
                    new IndexedNgramProperty(_indexedItems.Count - 1, groupedSyllable.NgramCount));

                pivotIndexItem.TotalCount += groupedSyllable.NgramCount;
            }
        }

        internal void RemoveBlackListItem(TKeyType id)
        {
            _ignoredItems.Add(id);
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
                         (searchedNgram, indexedNgram) => (double)Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount),
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
            if(reducePriorityOfNoisyNgrams)
            {
                return Search(searchedPhrase,

                             (searchedNgram, indexedNgram) => 
                                (double)Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount)
                                 / _pivotIndex[searchedNgram.Ngram].TotalCount,

                             (indexedItem, intersections, searchNgrams) => 
                                intersections / (indexedItem.GetReducedPriorityNoisyNgramCount(_pivotIndex) 
                                                 + searchNgrams.Sum(x => ((double)x.NgramCount) 
                                                                          / (_pivotIndex.ContainsKey(x.Ngram) ? _pivotIndex[x.Ngram].TotalCount: 1))));
            }

            return Search(searchedPhrase,
                         (searchedNgram, indexedNgram) => (double)Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount),
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
            return Search(searchedPhrase,
                         (searchedNgram, indexedNgram) => (double)Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount),
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
            return Search(searchedPhrase,
                         (searchedNgram, indexedNgram) => (double)Math.Min(searchedNgram.NgramCount, indexedNgram.NgramCount),
                         (indexedItem, intersections, searchNgrams) => 
                            intersections / (indexedItem.NgramCount + searchNgrams.Sum(x => x.NgramCount) - intersections));
        }

        

        private IEnumerable<ResultItem<TKeyType>> Search(string searchedPhrase,
                                                        CalculateNgramSimilarity calculateNgramSimilarity,
                                                        CalculatePhraseSimilarity calculatePhraseSimilarity)
        {
            var result = new List<ResultItem<TKeyType>>();

            IList<GroupedNgram> searchNgrams = CreateNgrams(searchedPhrase, NCount);

            var intersections = new Dictionary<int, double>(); // index item primary key, number of intersections

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

            result.AddRange(
                intersections.Keys.Select(
                    itemIndex =>
                        new ResultItem<TKeyType>
                        {
                            Id = _indexedItems[itemIndex].Id,
                            Similarity = calculatePhraseSimilarity(_indexedItems[itemIndex], intersections[itemIndex], searchNgrams)
                        }));


            return result.OrderByDescending(s => s.Similarity);
        }

        public IEnumerable<GroupedNgram> GetAllNgrams()
        {
            return _pivotIndex.Select(kvPair => new GroupedNgram { Ngram = kvPair.Key, NgramCount = kvPair.Value.TotalCount });
        }

        //private static string RemoveDiacritics(string s)
        //{
        //    s = s.Normalize(NormalizationForm.FormD).ToLower();
        //    var sb = new StringBuilder();

        //    foreach (char t in s)
        //    {
        //        if (CharUnicodeInfo.GetUnicodeCategory(t) != UnicodeCategory.NonSpacingMark) sb.Append(t);
        //    }

        //    return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"\s+", " ");
        //}


        private IList<GroupedNgram> CreateNgrams(string str, int n)
        {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(str))
            {
                var len = str.Length;
                for (int i = -1 * (n - (NCount - 1)); i < len - (NCount - 2); ++i)
                {
                    var sb = new StringBuilder();
                    var p = i;
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
