using System.Linq;
using Xunit;

namespace NGramSearchTest
{
    public class SorensenDiceCoefficientTest
    {
        [Fact]
        public void SorensenDiceEasyTest()
        {
            var index = new NGramSearch.SorensenDiceCoefficientIndex<int>();
            index.Add(1, "abcd"); // _ab abc bcd cd_

            var result = index.Search("ab"); // _ab ab_
            // 2 * 1 / (4 + 2) = 1 / 3
            Assert.Equal(1.0 / 3, result.Single().Similarity, 8);

            result = index.Search("abc"); // _ab abc bc_
            // 2 * 2  / (4 + 3) = 4 / 7
            Assert.Equal(4.0 / 7, result.Single().Similarity, 8);

            result = index.Search("abcd abcd"); // _ab abc bcd cd_ d_a _ab abc bcd cd_
            // 2 * 4 / (4 + 9) = 8 / 13
            Assert.Equal(8.0 / 13, result.Single().Similarity, 8);

            index.Add(2, "xyz xyz"); // _xy xyz yz_ z_x _xy xyz yz_
            result = index.Search("xyz"); // _xy xyz yz_
            // 2 * 3 / (7 + 3) = 0.6
            Assert.Equal(0.6, result.First().Similarity, 8);
        }

    }
}
