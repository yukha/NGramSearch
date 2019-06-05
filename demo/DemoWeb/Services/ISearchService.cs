using DemoWeb.Models;
using System.Collections.Generic;

namespace DemoWeb.Services
{
    public interface ISearchService
    {
        string Name { get; }

        IEnumerable<SearchResultLine> SearchWithIntersectionCount(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false);

        IEnumerable<SearchResultLine> SearchWithSorensenDiceCoefficient(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false);

        IEnumerable<SearchResultLine> SearchWithJaccardIndex(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false);
    }
}
