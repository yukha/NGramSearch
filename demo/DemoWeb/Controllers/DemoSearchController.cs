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
            
            return searchService.SearchWithIntersectionCount(searchRequest.SearchedPhrase);            
        }
    }
}
