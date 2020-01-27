using DemoWeb.Models;
using DemoWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DemoWeb.Controllers
{
    [Route("api/[controller]")]
    public class DemoSearchController : Controller
    {
        private readonly Func<string, IntersectionCountSearchService> _intersectionCountServiceAccessor;
        private readonly Func<string, SorensenDiceCoefficientSearchService> _sorensenDiceCoefficientServiceAccessor;
        private readonly Func<string, JaccardIndexSearchService> _jaccardIndexServiceAccessor;


        public DemoSearchController(
            Func<string, IntersectionCountSearchService> intersectionCountServiceAccessor,
            Func<string, SorensenDiceCoefficientSearchService> sorensenDiceCoefficientServiceAccessor,
            Func<string, JaccardIndexSearchService> jaccardIndexServiceAccessor )
        {
            _intersectionCountServiceAccessor = intersectionCountServiceAccessor;
            _sorensenDiceCoefficientServiceAccessor = sorensenDiceCoefficientServiceAccessor;
            _jaccardIndexServiceAccessor = jaccardIndexServiceAccessor;
        }

        [HttpPost("intersectionCount")]
        public IEnumerable<SearchResultLine> IntersectionCount([FromBody] SearchRequest searchRequest)
        {
            var searchService = _intersectionCountServiceAccessor(searchRequest.SourceType);
            return searchService.SearchWithIntersectionCount(searchRequest.SearchedPhrase);
        }

        [HttpPost("sorensenDiceCoefficient")]
        public IEnumerable<SearchResultLine> SorensenDiceCoefficient([FromBody] SearchRequest searchRequest)
        {
            var searchService = _jaccardIndexServiceAccessor(searchRequest.SourceType);
            return searchService.SearchWithSorensenDiceCoefficient(searchRequest.SearchedPhrase);
        }

        [HttpPost("jaccardIndex")]
        public IEnumerable<SearchResultLine> JaccardIndex([FromBody] SearchRequest searchRequest)
        {
            var searchService = _sorensenDiceCoefficientServiceAccessor(searchRequest.SourceType);
            return searchService.SearchWithJaccardIndex(searchRequest.SearchedPhrase);
        }

    }
}
