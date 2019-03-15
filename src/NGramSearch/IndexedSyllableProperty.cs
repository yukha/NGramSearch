namespace NGramSearch
{
    class IndexedSyllableProperty
    {
        public IndexedSyllableProperty(int wordIndex, int syllableCount)
        {
            WordIndex = wordIndex;
            SyllableCount = syllableCount;
        }
        public int WordIndex { get; }
        public int SyllableCount { get; }
    }
}
