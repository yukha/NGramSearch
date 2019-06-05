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

                case "sorensenDiceCoefficient":
                    return searchService.SearchWithSorensenDiceCoefficient(searchRequest.SearchedPhrase);

                case "jaccardIndex":
                    return searchService.SearchWithJaccardIndex(searchRequest.SearchedPhrase);
            }
            throw new Exception("search type is not implemented.");
        }
    }
}
