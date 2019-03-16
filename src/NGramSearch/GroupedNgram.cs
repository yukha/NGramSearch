namespace NGramSearch
{
    public class GroupedNgram
    {
        public string Ngram { get; set; }

        /// <summary>
        /// Count this Ngram in one token (indexed item or search phrase)
        /// </summary>
        public int NgramCount { get; set; }
    }
}
