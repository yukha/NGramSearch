using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var result = source.SearchNgramEasyCount("aaa");

            Assert.Single(result);
            Assert.Equal("first", result.First().Id);
            Assert.Equal(3, result.First().Similarity); // _aa aaa aa_
        }

        [Fact]
        public void TargetFrequency()
        {
            var source = new NGramSearch.NGramIndex<string>();
            source.Add("first", "aaaaaaa");
            source.Add("second", "aaabbbb");

            var result = source.SearchNramCountWithFrequency("aaa");


            Assert.Equal("first", result.First().Id);
            Assert.Equal(10.0 / 6, result.First().Similarity, 8); // _aa aaa aa_ -> 1/2 + 1/6 + 1 = (3 + 1 + 6) / 6 = 10/6 = 1.6666666...
            Assert.Equal(2.0 / 3, result.ToArray()[1].Similarity, 8); // _aa aaa -> 1/2 + 1/6 = 4/6 = 2/3 = 0.666666...
        }

        [Fact]
        public void LengthComparison()
        {
            var source = new NGramSearch.NGramIndex<string>();
            source.Add("first", "aaaa");
            source.Add("second", "bbbb");


            var result = source.SearchNgramLengthComparison("aaa");

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

            var result = source.SearchNgramEasyCount("aaaa");

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

            var result = source.SearchNgramEasyCount("aaaaa");

            Assert.Single(result);
            Assert.Equal("first", result.First().Id);
            Assert.Equal(5, result.First().Similarity); // _aa aaa aaa aaa aa_
        }

        [Fact]
        public void TargetFrequencyWithDuplicitiInSearch()
        {
            var source = new NGramSearch.NGramIndex<string>();
            source.Add("first", "aaa");
            source.Add("second", "aaabbbb");

            var result = source.SearchNramCountWithFrequency("aaaaaaa");


            Assert.Equal("first", result.First().Id);
            Assert.Equal(2.0, result.First().Similarity, 8); // _aa aaa aa_ -> 1/2 + 1/2 + 1 = 2
            Assert.Equal(1.0, result.ToArray()[1].Similarity, 8); // _aa aaa -> 1/2 + 1/2 = 1
        }

        [Fact]
        public void TargetFrequencyBothDuplicity()
        {
            var source = new NGramSearch.NGramIndex<string>();
            source.Add("first", "aaaaaaa");
            source.Add("second", "aaabbbb");

            var result = source.SearchNramCountWithFrequency("aaaa");


            Assert.Equal("first", result.First().Id);
            Assert.Equal(11.0 / 6, result.First().Similarity, 8); // _aa aaa aaa aa_ -> 1/2 + 2/6 + 1 = (3 + 2 + 6) / 6 = 11/6
            Assert.Equal(2.0 / 3, result.ToArray()[1].Similarity, 8); // _aa aaa -> 1/2 + 1/6 = 4/6 = 2/3
        }
    }
}
