export enum SearchSourceType {
  actors = 'actors',
  films = 'films',
}

export enum SearchIndexType {
  intersectionCount = 'intersectionCount',
  intersectionCountNoisy = 'intersectionCountNoisy',
  simpleMatchingCoefficient = 'simpleMatchingCoefficient',
  simpleMatchingCoefficientNoisy = 'simpleMatchingCoefficientNoisy',
  sorensenDiceCoefficient = 'sorensenDiceCoefficient',
  sorensenDiceCoefficientNoisy = 'sorensenDiceCoefficientNoisy',
  jaccardIndex = 'jaccardIndex',
  jaccardIndexNoisy = 'jaccardIndexNoisy',
}
