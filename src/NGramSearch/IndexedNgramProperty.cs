namespace NGramSearch
{
    class IndexedNgramProperty
    {
        public IndexedNgramProperty(int wordIndex, int ngramCount)
        {
            WordIndex = wordIndex;
            NgramCount = ngramCount;
        }
        public int WordIndex { get; }
        public int NgramCount { get; }
    }
}
