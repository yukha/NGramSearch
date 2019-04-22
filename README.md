# Approximate matching and approximate search
Substrings of length N are called n-grams. N-grams partially overlap.
For example, phrase `a little`,
when using trigrams, will split into 8 parts (for simplicity symbol _ means whitespace)
`_a_ a_l _li lit itt ttl tle le_`

Phrases are divided into n-grams when indexing and searching. The result is determined by the number of corresponding n-grams. Implemented several ways to assess the search result.

- **Intersection count** – suitable for example for implementing the auto-complete feature. In this method, attention is not paid to the correspondence of the length of the indexed and searched phrase. For example, when we search “little” results of “a little party never killed nobody” and “a little boy” will have the same weight.

The following methods compare not only the number of matches but also the number of mismatches and are more suitable for displaying search results (when the user has completely finished typing), than to implement the autocomplete function.
- **Sørensen–Dice coefficient** – [wikipedia](https://en.wikipedia.org/wiki/S%C3%B8rensen%E2%80%93Dice_coefficient)
- **Jaccard index** – [wikipedia](https://en.wikipedia.org/wiki/Jaccard_index)
