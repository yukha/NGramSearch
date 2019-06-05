using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NGramSearchTest
{
    public class AddRemoveToIndexTest : NGramSearch.NGramIndex<int> // for test protected method
    {
        public AddRemoveToIndexTest(int nCount = 3) : base(nCount)
        {
        }



        [Fact]
        public void AddTest()
        {
            var index = new AddRemoveToIndexTest(2);

            IEnumerable<NGramSearch.GroupedNgram> ngrams = index.GetAllNgrams();
            Assert.NotNull(ngrams);
            Assert.Empty(ngrams);

            index.Add(1, "abc"); // _a ab bc c_
            Assert.Equal(4, ngrams.Count());
            Assert.Equal(" a", ngrams.First().Ngram);
            Assert.Equal(1, ngrams.First().NgramCount);
            var result = index.SearchWithIntersectionCount("a"); // _a
            Assert.Equal(1, result.Single().Id);
            Assert.Equal(1, result.Single().Similarity);

            index.Add(2, "abc"); // _a ab bc c_
            Assert.Equal(4, ngrams.Count());
            Assert.Equal(" a", ngrams.First().Ngram);
            Assert.Equal(2, ngrams.First().NgramCount);

            index.Add(3, "abc"); // _a ab bc c_
            Assert.Equal(4, ngrams.Count());
            Assert.Equal(" a", ngrams.First().Ngram);
            Assert.Equal(3, ngrams.First().NgramCount);

            index.Add(3, "xyz"); // _x xy yz z_
            Assert.Equal(8, ngrams.Count());
            Assert.Equal(" a", ngrams.First().Ngram);
            Assert.Equal(3, ngrams.First().NgramCount);
        }

        [Fact]
        public void RemoveTest()
        {
            var index = new AddRemoveToIndexTest(4);

            IEnumerable<NGramSearch.GroupedNgram> ngrams = index.GetAllNgrams();
            Assert.NotNull(ngrams);
            Assert.Empty(ngrams);

            index.Add(1, "abc"); // _abc abc_
            ngrams = index.GetAllNgrams();
            Assert.Equal(2, ngrams.Count());
            Assert.Equal(" abc", ngrams.First().Ngram);
            Assert.Equal(1, ngrams.First().NgramCount);

            index.Add(2, "abc"); // _abc abc_
            ngrams = index.GetAllNgrams();
            Assert.Equal(2, ngrams.Count());
            Assert.Equal(" abc", ngrams.First().Ngram);
            Assert.Equal(2, ngrams.First().NgramCount);

            index.Remove(1);
            ngrams = index.GetAllNgrams();
            Assert.Equal(2, ngrams.Count());
            Assert.Equal(" abc", ngrams.First().Ngram);
            Assert.Equal(1, ngrams.First().NgramCount);

            index.Remove(2);
            ngrams = index.GetAllNgrams();
            Assert.NotNull(ngrams);
            Assert.Empty(ngrams);
        }
    }
}
