using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NGramSearchTest
{
    public class SimpleMatchingTest
    {
        [Fact]
        public void SimpleMatchingEasyTest()
        {
            var index = new NGramSearch.NGramIndex<int>();
            index.Add(1, "abcd"); // _ab abc bcd cd_

            var result = index.SearchWithSimpleMatchingCoefficient("ab"); // _ab ab_
            // 1 / (4 + 2) = 1 / 6
            Assert.Equal(1.0 / 6, result.Single().Similarity, 8);

            result = index.SearchWithSimpleMatchingCoefficient("abc"); // _ab abc bc_
            // 2  / (4 + 3) = 2 / 7
            Assert.Equal(2.0 / 7, result.Single().Similarity, 8);

            result = index.SearchWithSimpleMatchingCoefficient("abcd abcd"); // _ab abc bcd cd_ d_a _ab abc bcd cd_
            // 4 / (4 + 9) = 4 / 13
            Assert.Equal(4.0 / 13, result.Single().Similarity, 8);

            index.Add(2, "xyz xyz"); // _xy xyz yz_ z_x _xy xyz yz_
            result = index.SearchWithSimpleMatchingCoefficient("xyz"); // _xy xyz yz_
            // 3 / (7 + 3) = 0.3
            Assert.Equal(0.3, result.First().Similarity, 8);
        }

        [Fact]
        public void SimpleMatchingNoisyNgramTest()
        {
            var index = new NGramSearch.NGramIndex<int>();
            index.Add(1, "abcd"); // _ab abc bcd cd_
            index.Add(2, "abx"); // _ab abx bx_

            var result = index.SearchWithSimpleMatchingCoefficient("ab", true); // _ab ab_
            Assert.Equal(2, result.First().Id);
            // 0.5 / (2.5 + 1.5) = 0.5 / 4
            Assert.Equal(0.5 / 4, result.First().Similarity, 8);
            // 0.5 / (3.5 + 1.5) = 0.5 / 5
            Assert.Equal(0.5 / 5, result.ToList()[1].Similarity, 8);

            result = index.SearchWithSimpleMatchingCoefficient("abc", true); // _ab abc bc_
            Assert.Equal(1, result.First().Id);
            // 1.5  / (3.5 + 2.5) = 1.5 / 6
            Assert.Equal(1.5 / 6, result.First().Similarity, 8);

            result = index.SearchWithSimpleMatchingCoefficient("abcd abcd", true); // _ab abc bcd cd_ d_a _ab abc bcd cd_
            Assert.Equal(1, result.First().Id);
            // 3.5 / (3.5 + 8) = 3.5 / 11.5
            Assert.Equal(3.5 / 11.5, result.First().Similarity, 8);

            index.Add(3, "xyz xyz"); // _xy xyz yz_ z_x _xy xyz yz_
            result = index.SearchWithSimpleMatchingCoefficient("xyz", true); // _xy xyz yz_
            Assert.Equal(3, result.First().Id);
            // 1.5 / (4 + 1.5) = 1.5 / 5.5
            Assert.Equal(1.5 / 5.5, result.First().Similarity, 8);
        }
    }
}
