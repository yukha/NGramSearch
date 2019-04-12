using DemoWeb.Models;
using System;
using System.Collections.Generic;

namespace DemoWeb.Services
{
    public class ActorsSearchService : ISearchService
    {
        public string Name => "actors";

        public IEnumerable<SearchResultLine> SearchWithIntersectionCount(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            return new[]
            {
                new SearchResultLine { Similarity = 0.9, Result = Name + " asdf 9"},
                new SearchResultLine { Similarity = 0.8, Result = Name + " asdf 8"},
                new SearchResultLine { Similarity = 0.7, Result = Name + " asdf 7"},
                new SearchResultLine { Similarity = 0.6, Result = Name + " asdf 6"},
                new SearchResultLine { Similarity = 0.5, Result = Name + " asdf 5"}
            };
        }

        public IEnumerable<SearchResultLine> SearchWithJaccardIndex(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            return new[]
            {
                new SearchResultLine { Similarity = 0.9, Result = Name + " asdf 9"},
                new SearchResultLine { Similarity = 0.8, Result = Name + " asdf 8"},
                new SearchResultLine { Similarity = 0.7, Result = Name + " asdf 7"},
                new SearchResultLine { Similarity = 0.6, Result = Name + " asdf 6"},
                new SearchResultLine { Similarity = 0.5, Result = Name + " asdf 5"}
            };
        }

        public IEnumerable<SearchResultLine> SearchWithSimpleMatchingCoefficient(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            return new[]
            {
                new SearchResultLine { Similarity = 0.9, Result = Name + " asdf 9"},
                new SearchResultLine { Similarity = 0.8, Result = Name + " asdf 8"},
                new SearchResultLine { Similarity = 0.7, Result = Name + " asdf 7"},
                new SearchResultLine { Similarity = 0.6, Result = Name + " asdf 6"},
                new SearchResultLine { Similarity = 0.5, Result = Name + " asdf 5"}
            };
        }

        public IEnumerable<SearchResultLine> SearchWithSorensenDiceCoefficient(string searchedPhrase, bool reducePriorityOfNoisyNgrams = false)
        {
            return new[]
            {
                new SearchResultLine { Similarity = 0.9, Result = Name + " asdf 9"},
                new SearchResultLine { Similarity = 0.8, Result = Name + " asdf 8"},
                new SearchResultLine { Similarity = 0.7, Result = Name + " asdf 7"},
                new SearchResultLine { Similarity = 0.6, Result = Name + " asdf 6"},
                new SearchResultLine { Similarity = 0.5, Result = Name + " asdf 5"}
            };
        }
    }
}
