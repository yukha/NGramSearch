using System.Linq;
using Xunit;

namespace NGramSearchTest
{
    public class DuplicityInIndexedItemTest
    {
        [Fact]
        public void SimpleCount()
        {
            var source = new NGramSearch.NGramIndex<string>();
            source.Add("first", "aaaa");
            source.Add("second", "bbbb");

            var result = source.SearchWithIntersectionCount("aaa");

            Assert.Single(result);
            Assert.Equal("first", result.First().Id);
            Assert.Equal(3, result.First().Similarity); // _aa aaa aa_
        }


        [Fact]
        public void LengthComparison()
        {
            var source = new NGramSearch.NGramIndex<string>();
            source.Add("first", "aaaa");
            source.Add("second", "bbbb");


            var result = source.SearchWithSorensenDiceCoefficient("aaa");

            Assert.Single(result);
            Assert.Equal("first", result.First().Id);
            //Assert.Equal(3.0/4, result.First().Similarity, 8); // aaa contains 3, aaaa contains 4. 
        }

        [Fact]
        public void SimpleCountWithDuplicitiInSearch()
        {
            var source = new NGramSearch.NGramIndex<string>();
            source.Add("first", "aaa");
            source.Add("second", "bbb");

            var result = source.SearchWithIntersectionCount("aaaa");

            Assert.Single(result);
            Assert.Equal("first", result.First().Id);
            Assert.Equal(3, result.First().Similarity); // _aa aaa aaa aa_, but index has only 3
        }

        [Fact]
        public void SimpleCountBothDuplicity()
        {
            var source = new NGramSearch.NGramIndex<string>();
            source.Add("first", "aaaaaaaaaa");
            source.Add("second", "bbb");

            var result = source.SearchWithIntersectionCount("aaaaa");

            Assert.Single(result);
            Assert.Equal("first", result.First().Id);
            Assert.Equal(5, result.First().Similarity); // _aa aaa aaa aaa aa_
        }
    }
}
