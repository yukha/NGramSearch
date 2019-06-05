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
        private readonly Func<string, ISearchService> ServiceAccessor;

        public DemoSearchController(Func<string, ISearchService> serviceAccessor)
        {
            ServiceAccessor = serviceAccessor;
        }

        [HttpPost("search")]
        public IEnumerable<SearchResultLine> Search([FromBody] SearchRequest searchRequest)
        {
            ISearchService searchService = ServiceAccessor(searchRequest.SourceType);

            switch (searchRequest.SearchType)
            {
                case "intersectionCount":
                    return searchService.SearchWithIntersectionCount(searchRequest.SearchedPhrase);
                case "intersectionCountNoisy":
                    return searchService.SearchWithIntersectionCount(searchRequest.SearchedPhrase, true);
                case "sorensenDiceCoefficient":
                    return searchService.SearchWithSorensenDiceCoefficient(searchRequest.SearchedPhrase);
                case "sorensenDiceCoefficientNoisy":
                    return searchService.SearchWithSorensenDiceCoefficient(searchRequest.SearchedPhrase, true);
                case "jaccardIndex":
                    return searchService.SearchWithJaccardIndex(searchRequest.SearchedPhrase);
                case "jaccardIndexNoisy":
                    return searchService.SearchWithJaccardIndex(searchRequest.SearchedPhrase, true);
            }
            throw new Exception("search type is not implemented.");
        }
    }
}
