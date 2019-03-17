using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NGramSearch
{
    public class NGramIndex<TKeyType>
    {
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

        public IEnumerable<ResultItem<TKeyType>> SearchNgramEasyCount(string searchedPhrase)
        {
            return Search(searchedPhrase, (data) => Math.Min(data.searchNgramCount, data.indexedItemNgramCoun));
        }

        public IEnumerable<ResultItem<TKeyType>> SearchNramCountWithFrequency(string searchedPhrase)
        {
            return Search(searchedPhrase, (data) =>
            (double)Math.Min(data.searchNgramCount, data.indexedItemNgramCoun)
                        / data.indexNgramCount);
        }

        private IEnumerable<ResultItem<TKeyType>> Search(string searchedPhrase,
            Func<(int searchNgramCount, int indexedItemNgramCoun, int indexNgramCount), double> calculateSimilarity)
        {
            var result = new List<ResultItem<TKeyType>>();

            var searchNgrams = CreateNgrams(searchedPhrase, NCount);

            var wordIndexSimilarity = new Dictionary<int, double>();

            foreach (GroupedNgram groupedNgram in searchNgrams)
            {
                if (_pivotIndex.ContainsKey(groupedNgram.Ngram))
                {
                    foreach (var wordIndexAndCount in _pivotIndex[groupedNgram.Ngram].IndexedItemReferences)
                    {
                        if (!wordIndexSimilarity.ContainsKey(wordIndexAndCount.WordIndex))
                        {
                            wordIndexSimilarity[wordIndexAndCount.WordIndex]
                                = calculateSimilarity((groupedNgram.NgramCount, wordIndexAndCount.NgramCount, _pivotIndex[groupedNgram.Ngram].TotalCount));
                        }
                        else
                        {
                            wordIndexSimilarity[wordIndexAndCount.WordIndex]
                               += calculateSimilarity((groupedNgram.NgramCount, wordIndexAndCount.NgramCount, _pivotIndex[groupedNgram.Ngram].TotalCount));
                        }
                    }
                }
            }

            result.AddRange(
                wordIndexSimilarity.Keys.Select(
                    dkey =>
                        new ResultItem<TKeyType>
                        {
                            Id = _indexedItems[dkey].Id,
                            Similarity = wordIndexSimilarity[dkey],
                        }));


            return result.OrderByDescending(s => s.Similarity);
        }

        public IEnumerable<ResultItem<TKeyType>> SearchNgramLengthComparison(string searchedValue)
        {
            var result = new List<ResultItem<TKeyType>>();

            var tg = CreateNgrams(searchedValue, NCount);

            // deep clone dictionary
            var ngrams = _pivotIndex.ToDictionary(ngramItem => ngramItem.Key, ngramItem => ngramItem.Value.IndexedItemReferences.ToList());


            var foundTrigramsForSubject = new Dictionary<int, int>();
            foreach (var t in tg)
            {
                if (ngrams.ContainsKey(t.Ngram))
                {
                    int len = ngrams[t.Ngram].Count;
                    int lastSubjectPos = -1;
                    for (int i = 0; i < len; ++i)
                    {
                        int subjPos = ngrams[t.Ngram][i].WordIndex;
                        if (subjPos == lastSubjectPos) continue;

                        if (!foundTrigramsForSubject.ContainsKey(subjPos))
                        {
                            foundTrigramsForSubject[subjPos] = 1;
                        }
                        else
                        {
                            foundTrigramsForSubject[subjPos] += 1;
                        }
                        lastSubjectPos = subjPos;
                        ngrams[t.Ngram].RemoveAt(i);
                        --len;
                        --i;
                    }
                }
            }

            result.AddRange(
                foundTrigramsForSubject.Keys.Select(
                    dkey =>
                        new ResultItem<TKeyType>
                        {
                            Id = _indexedItems[dkey].Id,
                            Similarity = CompareLength(tg.Count, _indexedItems[dkey].Ngrams.Count, foundTrigramsForSubject[dkey])
                        }));


            return result.Where(r => !_ignoredItems.Contains(r.Id)).OrderByDescending(s => s.Similarity);
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

        private static double CompareLength(double sourceLength, double targetLength, double foundPieces)
        {
            return (foundPieces / sourceLength) * (foundPieces / targetLength);
        }

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
