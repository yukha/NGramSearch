using DemoWeb.Models;
using System.Collections.Generic;

namespace DemoWeb.Services
{
    public interface ISearchService
    {
        string Name { get; }

        IEnumerable<SearchResultLine> SearchWithIntersectionCount(string searchedPhrase);

        IEnumerable<SearchResultLine> SearchWithSorensenDiceCoefficient(string searchedPhrase);

        IEnumerable<SearchResultLine> SearchWithJaccardIndex(string searchedPhrase);
    }
}
