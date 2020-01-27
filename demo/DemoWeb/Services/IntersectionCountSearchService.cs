using DemoWeb.Models;
using System.Collections.Generic;
using System.Linq;
using NGramSearch;

namespace DemoWeb.Services
{
    public class IntersectionCountSearchService : JsonFileSource<IntersectionCountIndex<int>>, ISearchService
    {
        public string Name { get; }

        public IntersectionCountSearchService(string name)
        {
            Name = name;
        }

        public IEnumerable<SearchResultLine> SearchWithIntersectionCount(string searchedPhrase)
        {
            (NGramSearch.NGramIndex<int> index, Dictionary<int, string> items) = GetIndex(Name);

            return index.Search(searchedPhrase).Take(10).Select(x => new SearchResultLine
            {
                Similarity = x.Similarity,
                Result = items[x.Id]
            });

        }

        public IEnumerable<SearchResultLine> SearchWithJaccardIndex(string searchedPhrase)
        {
            (NGramSearch.NGramIndex<int> index, Dictionary<int, string> items) = GetIndex(Name);

            return index.Search(searchedPhrase).Take(10).Select(x => new SearchResultLine
            {
                Similarity = x.Similarity,
                Result = items[x.Id]
            });
        }

        public IEnumerable<SearchResultLine> SearchWithSorensenDiceCoefficient(string searchedPhrase)
        {
            (NGramSearch.NGramIndex<int> index, Dictionary<int, string> items) = GetIndex(Name);

            return index.Search(searchedPhrase).Take(10).Select(x => new SearchResultLine
            {
                Similarity = x.Similarity,
                Result = items[x.Id]
            });
        }
    }
}
