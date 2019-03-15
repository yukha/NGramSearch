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

        private readonly IDictionary<string, List<IndexedSyllableProperty>> _ngrams
            = new Dictionary<string, List<IndexedSyllableProperty>>();

        private readonly List<TKeyType> _ignoredItems = new List<TKeyType>();

        public NGramIndex() : this(3)
        {
        }

        public NGramIndex(int nCount)
        {
            NCount = nCount;
        }


        public void Add(TKeyType id, string normalizedValue)
        {

            var cachedItem = new IndexedItem<TKeyType> { Id = id, NormalizedValue = normalizedValue };
            cachedItem.Ngrams = CreateNgrams(cachedItem.NormalizedValue, NCount);

            _indexedItems.Add(cachedItem);


            foreach (GroupedSyllable groupedSyllable in cachedItem.Ngrams)
            {
                if (!_ngrams.ContainsKey(groupedSyllable.Syllable))
                {
                    _ngrams[groupedSyllable.Syllable] = new List<IndexedSyllableProperty>();
                }

                // add position to the list of positions
                _ngrams[groupedSyllable.Syllable].Add(
                    new IndexedSyllableProperty(_indexedItems.Count - 1, groupedSyllable.SyllableCount));
            }
        }

        internal void RemoveBlackListItem(TKeyType id)
        {
            _ignoredItems.Add(id);
        }

        public IEnumerable<ResultItem<TKeyType>> SearchNgramEasyCount(string searchedValue)
        {
            var result = new List<ResultItem<TKeyType>>();

            var searchedSyllables = CreateNgrams(searchedValue, NCount);

            var wordIndexSimilarity = new Dictionary<int, int>();

            foreach (GroupedSyllable searchedGroupedSyllable in searchedSyllables)
            {
                // HashSet<int> usedItem = new HashSet<int>();
                if (_ngrams.ContainsKey(searchedGroupedSyllable.Syllable))
                {
                    foreach (var wordIndexAndCount in _ngrams[searchedGroupedSyllable.Syllable])
                    {
                        // if (usedItem.Contains(indexedWordPosition)) continue;

                        if (!wordIndexSimilarity.ContainsKey(wordIndexAndCount.WordIndex))
                        {
                            wordIndexSimilarity[wordIndexAndCount.WordIndex] 
                                = Math.Min(searchedGroupedSyllable.SyllableCount, wordIndexAndCount.SyllableCount);
                        }
                        else
                        {
                            wordIndexSimilarity[wordIndexAndCount.WordIndex]
                                += Math.Min(searchedGroupedSyllable.SyllableCount, wordIndexAndCount.SyllableCount);
                        }

                        // usedItem.Add(indexedWordPosition);
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

        public IEnumerable<ResultItem<TKeyType>> SearchNramTargetFrequencyCoeff(string searchedValue)
        {
            var result = new List<ResultItem<TKeyType>>();

            var searchedSyllables = CreateNgrams(searchedValue, NCount);

            var wordIndexSimilarity = new Dictionary<int, double>();

            foreach (GroupedSyllable searchedGroupedSyllable in searchedSyllables)
            {
                // HashSet<int> usedItem = new HashSet<int>();
                if (_ngrams.ContainsKey(searchedGroupedSyllable.Syllable))
                {
                    foreach (var wordIndexAndCount in _ngrams[searchedGroupedSyllable.Syllable])
                    {
                        // if (usedItem.Contains(indexedWordPosition)) continue;

                        if (!wordIndexSimilarity.ContainsKey(wordIndexAndCount.WordIndex))
                        {
                            wordIndexSimilarity[wordIndexAndCount.WordIndex] 
                                = (double)Math.Min(searchedGroupedSyllable.SyllableCount, wordIndexAndCount.SyllableCount) 
                                  / (_ngrams[searchedGroupedSyllable.Syllable].Sum(x => x.SyllableCount));
                        }
                        else
                        {
                            wordIndexSimilarity[wordIndexAndCount.WordIndex] 
                                += (double)Math.Min(searchedGroupedSyllable.SyllableCount, wordIndexAndCount.SyllableCount) 
                                   / (_ngrams[searchedGroupedSyllable.Syllable].Sum(x => x.SyllableCount));
                        }

                        //usedItem.Add(indexedWordPosition);
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
            var ngrams = _ngrams.ToDictionary(ngramItem => ngramItem.Key, ngramItem => ngramItem.Value.ToList());


            var foundTrigramsForSubject = new Dictionary<int, int>();
            foreach (var t in tg)
            {
                if (ngrams.ContainsKey(t.Syllable))
                {
                    int len = ngrams[t.Syllable].Count;
                    int lastSubjectPos = -1;
                    for (int i = 0; i < len; ++i)
                    {
                        int subjPos = ngrams[t.Syllable][i].WordIndex;
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
                        ngrams[t.Syllable].RemoveAt(i);
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

        private IList<GroupedSyllable> CreateNgrams(string str, int n)
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

            return list.GroupBy(ngram => ngram).Select(g => new GroupedSyllable
            {
                Syllable = g.Key,
                SyllableCount = g.Count()
            }).ToList();
        }

    }
}
