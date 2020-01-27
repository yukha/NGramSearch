using System.Linq;
using Xunit;

namespace NGramSearchTest
{
    public class JaccardIndexTest
    {
        [Fact]
        public void JaccardIndexEasyTest()
        {
            var index = new NGramSearch.JaccardIndex<long>();
            index.Add(1, "abcd"); // _ab abc bcd cd_

            var result = index.Search("ab"); // _ab ab_
            // 1 / (4 + 2 - 1) = 1 / 5
            Assert.Equal(1.0 / 5, result.Single().Similarity, 8);

            result = index.Search("abc"); // _ab abc bc_
            // 2  / (4 + 3 - 2) = 2 / 5
            Assert.Equal(2.0 / 5, result.Single().Similarity, 8);

            result = index.Search("abcd abcd"); // _ab abc bcd cd_ d_a _ab abc bcd cd_
            // 4 / (4 + 9 - 4) = 4 / 9
            Assert.Equal(4.0 / 9, result.Single().Similarity, 8);

            index.Add(2, "xyz xyz"); // _xy xyz yz_ z_x _xy xyz yz_
            result = index.Search("xyz"); // _xy xyz yz_
            // 3 / (7 + 3 - 3) = 3 / 7
            Assert.Equal(3.0 / 7, result.First().Similarity, 8);
        }

    }
}
