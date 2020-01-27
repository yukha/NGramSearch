using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NGramSearchTest
{
    public class AddRemoveToIndexTest : NGramSearch.IntersectionCountIndex<int> // for test protected method
    {
        public AddRemoveToIndexTest(int nCount = 3) : base(nCount)
        {
        }



        [Fact]
        public void AddTest()
        {
            var index = new AddRemoveToIndexTest(2);

            IEnumerable<NGramSearch.GroupedNGram> ngrams = index.GetAllNGrams();
            Assert.NotNull(ngrams);
            Assert.Empty(ngrams);

            index.Add(1, "abc"); // _a ab bc c_
            Assert.Equal(4, ngrams.Count());
            Assert.Equal(" a", ngrams.First().NGram);
            Assert.Equal(1, ngrams.First().TotalPhraseNGramCount);
            var result = index.Search("a"); // _a
            Assert.Equal(1, result.Single().Id);
            Assert.Equal(1, result.Single().Similarity);

            index.Add(2, "abc"); // _a ab bc c_
            Assert.Equal(4, ngrams.Count());
            Assert.Equal(" a", ngrams.First().NGram);
            Assert.Equal(2, ngrams.First().TotalPhraseNGramCount);

            index.Add(3, "abc"); // _a ab bc c_
            Assert.Equal(4, ngrams.Count());
            Assert.Equal(" a", ngrams.First().NGram);
            Assert.Equal(3, ngrams.First().TotalPhraseNGramCount);

            index.Add(3, "xyz"); // _x xy yz z_
            Assert.Equal(8, ngrams.Count());
            Assert.Equal(" a", ngrams.First().NGram);
            Assert.Equal(3, ngrams.First().TotalPhraseNGramCount);
        }
    }
}
