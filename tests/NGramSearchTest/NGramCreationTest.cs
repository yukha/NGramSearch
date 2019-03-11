using System;
using Xunit;
using System.Linq;


namespace NGramSearchTest
{
    public class NGramCreationTest
    {
        [Fact]
        public void RobertHas6TriGramsTest()
        {
            var actors = new NGramSearch.NGramIndex<int>(3);
            actors.Add(1, "johnny depp");
            actors.Add(2, "al pacino");
            actors.Add(3, "robert de niro");
            actors.Add(4, "kevin spacey");


            var result = actors.SearchNgramEasyCount("robert");

            Assert.Single(result);
            Assert.Equal(3, result.First().Id);
            Assert.Equal(6, result.First().Similarity); // _ro rob obe ber ert rt_
        }

        [Fact]
        public void MeHas3BiGramTest()
        {
            var index = new NGramSearch.NGramIndex<long>(2);
            index.Add(1, "me");
            index.Add(2, "you");

            var result = index.SearchNgramEasyCount("me");

            Assert.Single(result);
            Assert.Equal(1, result.Single().Id);
            Assert.Equal(3, result.Single().Similarity); // _m me e_
        }

        [Fact]
        public void RobertHas5FourGrams()
        {
            var actors = new NGramSearch.NGramIndex<int>(4);
            actors.Add(1, "johnny depp");
            actors.Add(2, "al pacino");
            actors.Add(3, "robert de niro");
            actors.Add(4, "kevin spacey");


            var result = actors.SearchNgramEasyCount("robert");

            Assert.Single(result);
            Assert.Equal(3, result.First().Id);
            Assert.Equal(5, result.First().Similarity); // _rob robe ober bert ert_

        }
    }
}
