using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeIndexPerformaceConsoleTest
{
    public class IntersectionCountTest
    {
        private readonly NGramSearch.NGramIndex<int> Index;
        Dictionary<int, Actor> Data;

        public IntersectionCountTest(int ngramLength)
        {
            Index = new NGramSearch.NGramIndex<int>(ngramLength);
        }

        public void CreateIndex(IList<Actor> data)
        {
            Data = data.ToDictionary(x => x.Id, x => x);
            foreach (int key in Data.Keys)
            {
                Index.Add(key, Data[key].Name);
            }
        }

        public void RunSearch()
        {
            Console.WriteLine("SearchWithIntersectionCount:");
            IEnumerable<NGramSearch.ResultItem<int>> result = Index.SearchWithIntersectionCount("adam smith");
            PrintResult(result);
            Console.WriteLine();

            Console.WriteLine("SearchWithJaccardIndex:");
            result = Index.SearchWithJaccardIndex("adam smith");
            PrintResult(result);
            Console.WriteLine();

            Console.WriteLine("SearchWithSorensenDiceCoefficient:");
            result = Index.SearchWithSorensenDiceCoefficient("adam smith");
            PrintResult(result);
            Console.WriteLine();
        }

        private void PrintResult(IEnumerable<NGramSearch.ResultItem<int>> result)
        {
            result = result.Take(3);
            foreach (var item in result)
            {
                Console.WriteLine($"\tid: {item.Id}, similarity: {item.Similarity}, value: {Data[item.Id].Name}");
            }
        }
    }
}
