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

        [Fact]
        public void TargetFrequency()
        {
            var germaniFirms = new NGramSearch.NGramIndex<int>(3);
            germaniFirms.Add(1, "volkswagen ag");
            germaniFirms.Add(2, "daimler ag");
            germaniFirms.Add(3, "allianz se");
            germaniFirms.Add(4, "bmw ag");
            germaniFirms.Add(5, "siemens ag");
            germaniFirms.Add(6, "lange uhren gmbh");


            var result = germaniFirms.SearchNramCountWithFrequency(" se");

            Assert.Single(result);
            Assert.Equal(3, result.First().Id);
            Assert.Equal(2, result.First().Similarity); // _se se_

            result = germaniFirms.SearchNramCountWithFrequency(" ag");

            Assert.Equal(4, result.Count());
            Assert.All(result.Select(r => r.Id), item => (new[] { 1, 2, 4, 5 }).Contains(item)); // "ag" contains 1,2,4,5
            Assert.Equal(2.0 / 4, result.First().Similarity, 8); // _ag ag_ / 4 is count of result
        }

        [Fact]
        public void LengthComparison()
        {
            var germaniFirms = new NGramSearch.NGramIndex<int>();
            germaniFirms.Add(1, "volkswagen ag");
            germaniFirms.Add(2, "daimler ag");
            germaniFirms.Add(3, "allianz se");
            germaniFirms.Add(4, "bmw ag");
            germaniFirms.Add(5, "siemens ag");
            germaniFirms.Add(6, "lange uhren gmbh");


            var result = germaniFirms.SearchNgramLengthComparison("bmw ag");

            Assert.All(result.Select(r => r.Id), item => (new[] { 1, 2, 4, 5 }).Contains(item) ); // "ag" contains 1,2,4,5
            Assert.Equal(4, result.First().Id); // 4 must be first
            Assert.Equal(1, result.First().Similarity); // "bmw ag" has 100% similarity

            result = germaniFirms.SearchNgramLengthComparison("bmw");

            Assert.Single(result);
            Assert.Equal(4, result.First().Id);
            Assert.Equal(3.0 / 6, result.First().Similarity, 8); // _bm bmw mw_ / _bm bmw mw_ w_a _ag ag_ = 3/6
        }

        [Fact]
        public void ChechNgramCount()
        {
            var germaniFirms = new NGramSearch.NGramIndex<int>();
            germaniFirms.Add(1, "volkswagen ag");
            germaniFirms.Add(2, "daimler ag");
            germaniFirms.Add(3, "allianz se");
            germaniFirms.Add(4, "bmw ag");
            germaniFirms.Add(5, "siemens ag");
            germaniFirms.Add(6, "lange uhren gmbh");

            var result = germaniFirms.GetAllNgrams().OrderByDescending(x => x.NgramCount).OrderBy(x => x.Ngram).ToList();
            Assert.Equal(4, result.First().NgramCount);
            Assert.Equal(" ag", result.First().Ngram);
        }
    }
}
