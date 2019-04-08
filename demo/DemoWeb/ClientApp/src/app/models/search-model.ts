import { SearchSourceType } from '../search-source-type.enum';

export interface SearchResultLine {
  similarity: number;
  result: string;
}

export interface SearchBoxStateModel {
  hidden: boolean;
  searchResult: SearchResultLine[];
}

export interface SearchStateModel {
  sourceType: SearchSourceType;
  searchedPhrase: string;

  intersectionCount: SearchBoxStateModel;
  intersectionCountNoisy: SearchBoxStateModel;
  simpleMatchingCoefficient: SearchBoxStateModel;
  simpleMatchingCoefficientNoisy: SearchBoxStateModel;
  sorensenDiceCoefficient: SearchBoxStateModel;
  sorensenDiceCoefficientNoisy: SearchBoxStateModel;
  jaccardIndex: SearchBoxStateModel;
  jaccardIndexNoisy: SearchBoxStateModel;
}
