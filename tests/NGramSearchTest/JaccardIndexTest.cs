using System.Linq;
using Xunit;

namespace NGramSearchTest
{
    public class JaccardIndexTest
    {
        [Fact]
        public void JaccardIndexEasyTest()
        {
            var index = new NGramSearch.NGramIndex<int>();
            index.Add(1, "abcd"); // _ab abc bcd cd_

            var result = index.SearchWithJaccardIndex("ab"); // _ab ab_
            // 1 / (4 + 2 - 1) = 1 / 5
            Assert.Equal(1.0 / 5, result.Single().Similarity, 8);

            result = index.SearchWithJaccardIndex("abc"); // _ab abc bc_
            // 2  / (4 + 3 - 2) = 2 / 5
            Assert.Equal(2.0 / 5, result.Single().Similarity, 8);

            result = index.SearchWithJaccardIndex("abcd abcd"); // _ab abc bcd cd_ d_a _ab abc bcd cd_
            // 4 / (4 + 9 - 4) = 4 / 9
            Assert.Equal(4.0 / 9, result.Single().Similarity, 8);

            index.Add(2, "xyz xyz"); // _xy xyz yz_ z_x _xy xyz yz_
            result = index.SearchWithJaccardIndex("xyz"); // _xy xyz yz_
            // 3 / (7 + 3 - 3) = 3 / 7
            Assert.Equal(3.0 / 7, result.First().Similarity, 8);
        }

        [Fact]
        public void JaccardIndexNoisyNgramTest()
        {
            var index = new NGramSearch.NGramIndex<int>();
            index.Add(1, "abcd"); // _ab abc bcd cd_
            index.Add(2, "abx"); // _ab abx bx_

            var result = index.SearchWithJaccardIndex("ab", true); // _ab ab_
            Assert.Equal(2, result.First().Id);
            // 0.5 / (2.5 + 1.5 - 0.5) = 0.5 / 3.5
            Assert.Equal(0.5 / 3.5, result.First().Similarity, 8);
            // 0.5 / (3.5 + 1.5 - 0.5) = 0.5 / 4.5
            Assert.Equal(0.5 / 4.5, result.ToList()[1].Similarity, 8);

            result = index.SearchWithJaccardIndex("abc", true); // _ab abc bc_
            Assert.Equal(1, result.First().Id);
            // 1.5  / (3.5 + 2.5 - 1.5) = 1.5 / 4.5
            Assert.Equal(1.5 / 4.5, result.First().Similarity, 8);
            // 0.5  / (2.5 + 2.5 - 0.5) = 0.5 / 4.5
            Assert.Equal(0.5 / 4.5, result.ToList()[1].Similarity, 8);

            result = index.SearchWithJaccardIndex("abcd abcd", true); // _ab abc bcd cd_ d_a _ab abc bcd cd_
            // 3.5 / (3.5 + 8 - 3.5) = 3.5 / 8
            Assert.Equal(3.5 / 8, result.First().Similarity, 8);
            // 0.5 / (2.5 + 8 - 0.5) = 0.5 / 10
            Assert.Equal(0.5 / 10, result.ToList()[1].Similarity, 8);

            index.Add(2, "xyz xyz"); // _xy xyz yz_ z_x _xy xyz yz_
            result = index.SearchWithJaccardIndex("xyz", true); // _xy xyz yz_
            // 1.5 / (4 + 1.5 - 1.5) = 1.5 / 4
            Assert.Equal(1.5 / 4, result.First().Similarity, 8);
        }
    }
}
