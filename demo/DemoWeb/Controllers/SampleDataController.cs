using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoWeb.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        [HttpPost("search")]
        public IEnumerable<SearchResultLine> Search()
        {
            return new[]
            {
                new SearchResultLine { Similarity = 1, Result = "asdf 1"},
                new SearchResultLine { Similarity = 0.9, Result = "asdf 9"},
                new SearchResultLine { Similarity = 0.8, Result = "asdf 8"},
                new SearchResultLine { Similarity = 0.7, Result = "asdf 7"},
                new SearchResultLine { Similarity = 0.6, Result = "asdf 6"},
                new SearchResultLine { Similarity = 0.5, Result = "asdf 5"},
                new SearchResultLine { Similarity = 0.4, Result = "asdf 4"},
                new SearchResultLine { Similarity = 0.3, Result = "asdf 3"},
                new SearchResultLine { Similarity = 0.2, Result = "asdf 2"},
                new SearchResultLine { Similarity = 0.1, Result = "asdf 1"},
                new SearchResultLine { Similarity = 0.0, Result = "asdf 0"},
            };
        }

        public class SearchResultLine
        {
            public double Similarity { get; set; }
            public string Result { get; set; }
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }
}
