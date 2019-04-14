import { SearchIndexType, SearchSourceType } from '../search-source-type.enum';

export enum ActionType {
  SET_SOURCE_TYPE = '[MENU] set source type',
  SET_SEARCHED_PHRASE = '[SEARCH_BOX] set searched phrase',
  SEND_SEARCH_REQUEST = '[SET_SEARCHED_PHRASE_ACTION] send search request',
  SET_METHOD_RESULT_VISIBLE = '[SETTING_DIALOG] set search method result visible',
}

export class SetSourceType {
  public static readonly type = ActionType.SET_SOURCE_TYPE;
  constructor(public payload: SearchSourceType) {}
}

export class SetSearchedPhrase {
  public static readonly type = ActionType.SET_SEARCHED_PHRASE;
  constructor(public payload: string) {}
}

export class SendSearchRequest {
  public static readonly type = ActionType.SEND_SEARCH_REQUEST;
  constructor(
    public payload: {
      sourceType: SearchSourceType;
      searchedPhrase: string;
      searchType: SearchIndexType;
    }
  ) {}
}

export class SetMethodResultVisible {
  public static readonly type = ActionType.SET_METHOD_RESULT_VISIBLE;
  constructor(public payload: { sourceType: SearchSourceType; searchTypeVisible: boolean }) {}
}
