using System;
using System.Linq;
using Xunit;


namespace NGramSearchTest
{
    public class NGramCreationTest // : NGramSearch.IntersectionCountIndex<int> // for test protected method
    {
        //public NGramCreationTest(int nCount = 3) : base(nCount)
        //{
        //}

        [Fact]
        public void RobertHas6TriGramsTest()
        {
            var actors = new NGramSearch.IntersectionCountIndex<int>();
            actors.Add(1, "johnny depp");
            actors.Add(2, "al pacino");
            actors.Add(3, "robert de niro");
            actors.Add(4, "kevin spacey");


            var result = actors.Search("robert");

            Assert.Single(result);
            Assert.Equal(3, result.First().Id);
            Assert.Equal(6, result.First().Similarity); // _ro rob obe ber ert rt_
        }

        [Fact]
        public void MeHas3BiGramTest()
        {
            var index = new NGramSearch.IntersectionCountIndex<long>(2);
            index.Add(1, "me");
            index.Add(2, "you");

            var result = index.Search("me");

            Assert.Single(result);
            Assert.Equal(1, result.Single().Id);
            Assert.Equal(3, result.Single().Similarity); // _m me e_
        }

        [Fact]
        public void RobertHas5FourGrams()
        {
            var actors = new NGramSearch.IntersectionCountIndex<int>(4);
            actors.Add(1, "johnny depp");
            actors.Add(2, "al pacino");
            actors.Add(3, "robert de niro");
            actors.Add(4, "kevin spacey");


            var result = actors.Search("robert");

            Assert.Single(result);
            Assert.Equal(3, result.First().Id);
            Assert.Equal(5, result.First().Similarity); // _rob robe ober bert ert_
        }


        [Fact]
        public void LengthComparison()
        {
            var germaniFirms = new NGramSearch.SorensenDiceCoefficientIndex<int>();
            germaniFirms.Add(1, "volkswagen ag");
            germaniFirms.Add(2, "daimler ag");
            germaniFirms.Add(3, "allianz se");
            germaniFirms.Add(4, "bmw ag");
            germaniFirms.Add(5, "siemens ag");
            germaniFirms.Add(6, "lange uhren gmbh");


            var result = germaniFirms.Search("bmw ag");

            Assert.All(result.Select(r => r.Id), item => (new[] { 1, 2, 4, 5 }).Contains(item)); // "ag" contains 1,2,4,5
            Assert.Equal(4, result.First().Id); // 4 must be first
            Assert.Equal(1, result.First().Similarity); // "bmw ag" has 100% similarity

            result = germaniFirms.Search("bmw");

            Assert.Single(result);
            Assert.Equal(4, result.First().Id);
            // _bm bmw mw_ & _bm bmw mw_ w_a _ag ag_ : 2 * 3:intersection / (3 + 6) = 2 / 3
            Assert.Equal(2.0 / 3, result.First().Similarity, 8);
        }

        [Fact]
        public void CheckNgramCount()
        {
            var germaniFirms = new NGramSearch.IntersectionCountIndex<int>();
            germaniFirms.Add(1, "volkswagen ag");
            germaniFirms.Add(2, "daimler ag");
            germaniFirms.Add(3, "allianz se");
            germaniFirms.Add(4, "bmw ag");
            germaniFirms.Add(5, "siemens ag");
            germaniFirms.Add(6, "lange uhren gmbh");

            var result = germaniFirms.GetAllNGrams()
                .OrderByDescending(x => x.TotalPhraseNGramCount)
                .ThenBy(x => x.NGram)
                .ToList();

            Assert.Equal(4, result.First().TotalPhraseNGramCount);

            Assert.Equal(" ag", result[0].NGram);
            Assert.Equal(4, result[1].TotalPhraseNGramCount);

            Assert.Equal("ag ", result[1].NGram);
            Assert.Equal(4, result[1].TotalPhraseNGramCount);
        }

        [Fact]
        public void CheckTotalBigramCount()
        {
            var index = new NGramSearch.IntersectionCountIndex<int>(2);

            index.Add(1, "abcd");
            Assert.Equal(5, index.GetAllNGrams().Count());

            index.Add(2, "defg");
            Assert.Equal(10, index.GetAllNGrams().Count());

            index.Add(3, "abc"); // new is "c_"
            Assert.Equal(11, index.GetAllNGrams().Count());
        }

        [Fact]
        public void CheckTotalTrigramCount()
        {
            var index = new NGramSearch.IntersectionCountIndex<int>(3);

            index.Add(1, "abcd");
            Assert.Equal(4, index.GetAllNGrams().Count());

            index.Add(2, "defg");
            Assert.Equal(8, index.GetAllNGrams().Count());

            index.Add(3, "abc"); // new is "bc_"
            Assert.Equal(9, index.GetAllNGrams().Count());
        }

        [Fact]
        public void CheckTotalFourgramCount()
        {
            var index = new NGramSearch.IntersectionCountIndex<int>(4);

            index.Add(1, "abcd");
            Assert.Equal(3, index.GetAllNGrams().Count());

            index.Add(2, "defg");
            Assert.Equal(6, index.GetAllNGrams().Count());

            index.Add(3, "abc"); // new is "abc_"
            Assert.Equal(7, index.GetAllNGrams().Count());
        }

        [Fact]
        public void CheckTooShortWord()
        {
            var index = new NGramSearch.IntersectionCountIndex<int>(4);

            index.Add(1, "a");

            Assert.Empty(index.GetAllNGrams());
        }
    }
}
