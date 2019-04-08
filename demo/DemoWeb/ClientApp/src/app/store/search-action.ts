import { SearchSourceType } from '../search-source-type.enum';

export enum ActionType {
  SET_SOURCE_TYPE = '[MENU] set source type',
  SET_SEARCHED_PHRASE = '[SEARCH_BOX] set searched phrase',
}

export class SetSourceType {
  public static readonly type = ActionType.SET_SOURCE_TYPE;
  constructor(public payload: SearchSourceType) {}
}

export class SetSearchedPhrase {
  public static readonly type = ActionType.SET_SEARCHED_PHRASE;
  constructor(public payload: string) {}
}
