namespace DemoWeb.Models
{
    public class SearchRequest
    {
        public string SourceType { get; set; }
        public string SearchedPhrase { get; set; }
        public string IndexType { get; set; }
    }
}
