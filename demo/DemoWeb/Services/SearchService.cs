using DemoWeb.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoWeb.Services
{
    public class SearchService : JsonFileSource, ISearchService
    {
        public string Name { get; }

        public SearchService(string name)
        {
            Name = name;
        }

        public IEnumerable<SearchResultLine> SearchWithIntersectionCount(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            (NGramSearch.NGramIndex<int> index, Dictionary<int, string> items) = GetIndex(Name);

            return index.SearchWithIntersectionCount(searchedPhrase, reducePriorityOfNoisyNgrams).Take(5).Select(x => new SearchResultLine
            {
                Similarity = x.Similarity,
                Result = items[x.Id]
            });

        }

        public IEnumerable<SearchResultLine> SearchWithJaccardIndex(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            (NGramSearch.NGramIndex<int> index, Dictionary<int, string> items) = GetIndex(Name);

            return index.SearchWithJaccardIndex(searchedPhrase, reducePriorityOfNoisyNgrams).Take(5).Select(x => new SearchResultLine
            {
                Similarity = x.Similarity,
                Result = items[x.Id]
            });
        }

        public IEnumerable<SearchResultLine> SearchWithSimpleMatchingCoefficient(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            (NGramSearch.NGramIndex<int> index, Dictionary<int, string> items) = GetIndex(Name);

            return index.SearchWithSimpleMatchingCoefficient(searchedPhrase, reducePriorityOfNoisyNgrams).Take(5).Select(x => new SearchResultLine
            {
                Similarity = x.Similarity,
                Result = items[x.Id]
            });
        }

        public IEnumerable<SearchResultLine> SearchWithSorensenDiceCoefficient(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            (NGramSearch.NGramIndex<int> index, Dictionary<int, string> items) = GetIndex(Name);

            return index.SearchWithSorensenDiceCoefficient(searchedPhrase, reducePriorityOfNoisyNgrams).Take(5).Select(x => new SearchResultLine
            {
                Similarity = x.Similarity,
                Result = items[x.Id]
            });
        }
    }
}
