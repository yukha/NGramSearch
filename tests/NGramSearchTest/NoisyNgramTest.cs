using System.Linq;
using Xunit;

namespace NGramSearchTest
{
    public class NoisyNgramTest
    {
        [Fact]
        public void NoisyWordTest()
        {
            var index = new NGramSearch.NGramIndex<char>(3);
            index.Add('a', "abc ag");
            index.Add('b', "bmw ag");
            index.Add('c', "test ag");

            var abcResult = index.SearchWithIntersectionCount("abc", true);
            Assert.Equal('a', abcResult.First().Id);
            Assert.Equal(3, abcResult.First().Similarity); // "_ab", "abc", "bc_"

            var agResult = index.SearchWithIntersectionCount("ag", true);
            Assert.Equal(3, agResult.Count());
            Assert.Equal(2.0 / 3, agResult.First().Similarity, 8); // "_ag", "ag_" -> 1/3 + 1/3 = 2/3
        }

        [Fact]
        public void NoisyWordInOneItemTest()
        {
            var index = new NGramSearch.NGramIndex<int>(3);
            index.Add(1, "aggregator ag");
            var agResult = index.SearchWithIntersectionCount("agx", true);//  "_ag"  -> 1/2

            Assert.Equal(0.5, agResult.First().Similarity, 8);

            var ag2Result = index.SearchWithIntersectionCount("agx agy", true); // "_ag" -> 2/2
            Assert.Equal(1.0, ag2Result.First().Similarity, 8);
        }

        [Fact]
        public void TwoNoisyWordInItemTest()
        {
            var index = new NGramSearch.NGramIndex<char>(3);
            index.Add('a', "abc ag");
            index.Add('b', "bmw ag");
            index.Add('c', "test ag");
            index.Add('d', "aggregator ag"); // "_ag" x2

            var agResult = index.SearchWithIntersectionCount("ag", true);
            Assert.Equal(4, agResult.Count());
            Assert.Equal(9.0 / 20, agResult.First().Similarity, 8); // "_ag", "ag_" -> 1/5 + 1/4 = 9/20


            var ag2Result = index.SearchWithIntersectionCount("agx agy", true);
            Assert.Equal(4, ag2Result.Count());
            Assert.Equal('d', ag2Result.First().Id);

            // first reslt is "aggregator ag"
            Assert.Equal(0.4, ag2Result.First().Similarity, 8); // "_ag" -> 2/5

            // second reslt is 'a' or 'b' or 'c' - same Similarity
            Assert.Equal(0.2, ag2Result.ToList()[1].Similarity, 8); // "_ag" -> 1/5
        }
    }
}
