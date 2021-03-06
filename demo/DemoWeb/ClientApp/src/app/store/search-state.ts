import { Action, Selector, State, StateContext } from '@ngxs/store';
import { patch } from '@ngxs/store/operators';
import produce from 'immer';
import { tap } from 'rxjs/operators';
import { SearchResultLine, SearchStateModel } from '../models/search-model';
import { SearchIndexType } from '../search-source-type.enum';
import { ApiServiceService } from '../services/api-service.service';
import { SendSearchRequest, SetMethodResultVisible, SetSearchedPhrase, SetSourceType } from './search-action';

@State<SearchStateModel>({
  name: 'store',
  defaults: {
    sourceType: undefined,
    searchedPhrase: '',

    intersectionCount: { hidden: false, searchResult: [] },
    sorensenDiceCoefficient: { hidden: false, searchResult: [] },
    jaccardIndex: { hidden: false, searchResult: [] }
  }
})
export class SearchState {
  constructor(private api: ApiServiceService) {}

  @Selector()
  static getIntersectionCountHidden(state: SearchStateModel) {
    return state.intersectionCount.hidden;
  }

  @Selector()
  static getIntersectionCountResult(state: SearchStateModel) {
    return state.intersectionCount.searchResult;
  }

  @Selector()
  static getSorensenDiceCoefficientHidden(state: SearchStateModel) {
    return state.sorensenDiceCoefficient.hidden;
  }

  @Selector()
  static getSorensenDiceCoefficientResult(state: SearchStateModel) {
    return state.sorensenDiceCoefficient.searchResult;
  }

  @Selector()
  static getJaccardIndexHidden(state: SearchStateModel) {
    return state.jaccardIndex.hidden;
  }

  @Selector()
  static getJaccardIndexResult(state: SearchStateModel) {
    return state.jaccardIndex.searchResult;
  }

  @Action(SetSourceType)
  setSourceType(ctx: StateContext<SearchStateModel>, { payload }: SetSourceType) {
    // refresh whole model
    ctx.setState({
      sourceType: payload,
      searchedPhrase: '',

      intersectionCount: { hidden: false, searchResult: [] },
      sorensenDiceCoefficient: { hidden: false, searchResult: [] },
      jaccardIndex: { hidden: false, searchResult: [] }
    });
  }

  @Action(SetSearchedPhrase)
  setSearchedPhrase(ctx: StateContext<SearchStateModel>, { payload }: SetSearchedPhrase) {
    ctx.setState(
      patch<SearchStateModel>({
        searchedPhrase: payload
      })
    );

    const state = ctx.getState();
    Object.values(SearchIndexType)
      .filter((searchType: SearchIndexType) => !state[searchType].hidden)
      .forEach((searchType: SearchIndexType) => {
        ctx.dispatch(
          new SendSearchRequest({
            sourceType: state.sourceType,
            searchedPhrase: state.searchedPhrase,
            searchType
          })
        );
      });
  }

  @Action(SendSearchRequest)
  sendSearchRequest(ctx: StateContext<SearchStateModel>, { payload }: SendSearchRequest) {
    // ngxs will subscribe to return value
    return this.api.post('/api/demoSearch/' + payload.searchType, {}, payload).pipe(
      tap((lines: SearchResultLine[]) => {
        // side effect, using 'immer'
        ctx.setState(
          produce(draft => {
            draft[payload.searchType].searchResult = lines;
          })
        );
      })
    );
  }

  @Action(SetMethodResultVisible)
  setMethodResultVisible(ctx: StateContext<SearchStateModel>, { payload }: SetMethodResultVisible) {
    ctx.setState(
      produce(draft => {
        draft[payload.sourceType].hidden = !payload.searchTypeVisible;
      })
    );
  }
}
