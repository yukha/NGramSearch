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

        /// <summary>
        /// Dictionary (ngram, list of positions in _IndexedItems)
        /// </summary>
        private readonly IDictionary<string, List<int>> _ngrams = new Dictionary<string, List<int>>();

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


            foreach (var nGram in cachedItem.Ngrams)
            {
                if (!_ngrams.ContainsKey(nGram))
                {
                    _ngrams[nGram] = new List<int>();
                }

                // add position to the list of positions
                _ngrams[nGram].Add(_indexedItems.Count - 1);
            }
        }

        internal void RemoveBlackListItem(TKeyType id)
        {
            _ignoredItems.Add(id);
        }

        public IEnumerable<ResultItem<TKeyType>> SearchNgramEasyCount(string normalizedValue)
        {
            var result = new List<ResultItem<TKeyType>>();

            var tg = CreateNgrams(normalizedValue, NCount);

            var d = new Dictionary<int, int>();
            foreach (var t in tg)
            {
                if (_ngrams.ContainsKey(t))
                {
                    foreach (var subjPos in _ngrams[t])
                    {
                        if (!d.ContainsKey(subjPos))
                        {
                            d[subjPos] = 1;
                        }
                        else
                        {
                            d[subjPos]++;
                        }
                    }
                }
            }

            result.AddRange(
                d.Keys.Select(
                    dkey =>
                        new ResultItem<TKeyType>
                        {
                            Id = _indexedItems[dkey].Id,
                            Similarity = d[dkey],
                        }));


            return result.OrderByDescending(s => s.Similarity);
        }

        public IEnumerable<ResultItem<TKeyType>> SearchNramTargetFrequencyCoeff(string val)
        {
            var result = new List<ResultItem<TKeyType>>();

            var tg = CreateNgrams(val, NCount);

            var d = new Dictionary<int, double>();
            foreach (var t in tg)
            {
                if (_ngrams.ContainsKey(t))
                {
                    foreach (var subjPos in _ngrams[t])
                    {
                        if (!d.ContainsKey(subjPos))
                        {
                            d[subjPos] = (double)1 / (_ngrams[t].Count);
                        }
                        else
                        {
                            d[subjPos] += (double)1 / (_ngrams[t].Count);
                        }
                    }
                }
            }

            result.AddRange(
                d.Keys.Select(
                    dkey =>
                        new ResultItem<TKeyType>
                        {
                            Id = _indexedItems[dkey].Id,
                            Similarity = d[dkey],
                        }));


            return result.OrderByDescending(s => s.Similarity);
        }

        public IEnumerable<ResultItem<TKeyType>> SearchNgramLengthComparison(string val)
        {
            var result = new List<ResultItem<TKeyType>>();

            var tg = CreateNgrams(val, NCount);

            // deep clone dictionary
            var ngrams = _ngrams.ToDictionary(ngramItem => ngramItem.Key, ngramItem => ngramItem.Value.ToList());


            var foundTrigramsForSubject = new Dictionary<int, int>(); // <subject, number of found>
            foreach (var t in tg)
            {
                if (ngrams.ContainsKey(t))
                {
                    int len = ngrams[t].Count;
                    int lastSubjectPos = -1;
                    for (int i = 0; i < len; ++i)
                    {
                        int subjPos = ngrams[t][i];
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
                        ngrams[t].RemoveAt(i);
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

        private IList<string> CreateNgrams(string str, int n)
        {
            var list = new List<string>();
            if (!String.IsNullOrWhiteSpace(str))
            {
                var len = str.Length;
                for (int i = -1 * (n - (NCount - 1)); i < len-(NCount - 2); ++i)
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
            return list;
        }

    }
}
