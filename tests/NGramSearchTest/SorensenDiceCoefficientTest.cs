using System.Linq;
using Xunit;

namespace NGramSearchTest
{
    public class SorensenDiceCoefficientTest
    {
        [Fact]
        public void SorensenDiceEasyTest()
        {
            var index = new NGramSearch.NGramIndex<int>();
            index.Add(1, "abcd"); // _ab abc bcd cd_

            var result = index.SearchWithSorensenDiceCoefficient("ab"); // _ab ab_
            // 2 * 1 / (4 + 2) = 1 / 3
            Assert.Equal(1.0 / 3, result.Single().Similarity, 8);

            result = index.SearchWithSorensenDiceCoefficient("abc"); // _ab abc bc_
            // 2 * 2  / (4 + 3) = 4 / 7
            Assert.Equal(4.0 / 7, result.Single().Similarity, 8);

            result = index.SearchWithSorensenDiceCoefficient("abcd abcd"); // _ab abc bcd cd_ d_a _ab abc bcd cd_
            // 2 * 4 / (4 + 9) = 8 / 13
            Assert.Equal(8.0 / 13, result.Single().Similarity, 8);

            index.Add(2, "xyz xyz"); // _xy xyz yz_ z_x _xy xyz yz_
            result = index.SearchWithSorensenDiceCoefficient("xyz"); // _xy xyz yz_
            // 2 * 3 / (7 + 3) = 0.6
            Assert.Equal(0.6, result.First().Similarity, 8);
        }


        [Fact]
        public void SorensenDiceNoisyNgramTest()
        {
            var index = new NGramSearch.NGramIndex<int>();
            index.Add(1, "abcd"); // _ab abc bcd cd_
            index.Add(2, "abx"); // _ab abx bx_

            var result = index.SearchWithSorensenDiceCoefficient("ab", true); // _ab ab_
            Assert.Equal(2, result.First().Id);
            // 2 * 0.5 / (2.5 + 1.5) = 1 / 4
            Assert.Equal(1.0 / 4, result.First().Similarity, 8);
            // 2 * 0.5 / (3.5 + 1.5) = 1 / 5
            Assert.Equal(1.0 / 5, result.ToList()[1].Similarity, 8);

            result = index.SearchWithSorensenDiceCoefficient("abc", true); // _ab abc bc_
            // 2 * 1.5  / (3.5 + 2.5) = 3 / 6
            Assert.Equal(3.0 / 6, result.First().Similarity, 8);
            // 2 * 0.5  / (2.5 + 2.5) = 1 / 5
            Assert.Equal(1.0 / 5, result.ToList()[1].Similarity, 8);

            result = index.SearchWithSorensenDiceCoefficient("abcd abcd", true); // _ab abc bcd cd_ d_a _ab abc bcd cd_
            // 2 * 3.5 / (3.5 + 8) = 7 / 11.5
            Assert.Equal(7.0 / 11.5, result.First().Similarity, 8);
            // 2 * 0.5 / (2.5 + 8) = 1 / 10.5
            Assert.Equal(1.0 / 10.5, result.ToList()[1].Similarity, 8);

            index.Add(2, "xyz xyz"); // _xy xyz yz_ z_x _xy xyz yz_
            result = index.SearchWithSorensenDiceCoefficient("xyz", true); // _xy xyz yz_
            // 2 * 1.5 / (4 + 1.5) = 3 / 5.5
            Assert.Equal(3.0 / 5.5, result.First().Similarity, 8);
        }
    }
}
