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
    }
}
