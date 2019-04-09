import { Action, Selector, State, StateContext } from '@ngxs/store';
import { patch } from '@ngxs/store/operators';
import { SearchStateModel } from '../models/search-model';
import { SearchIndexType } from '../search-source-type.enum';
import { SendSearchRequest, SetSearchedPhrase, SetSourceType } from './search-action';

@State<SearchStateModel>({
  name: 'store',
  defaults: {
    sourceType: undefined,
    searchedPhrase: '',

    intersectionCount: { hidden: false, searchResult: [] },
    intersectionCountNoisy: { hidden: false, searchResult: [] },
    simpleMatchingCoefficient: { hidden: false, searchResult: [] },
    simpleMatchingCoefficientNoisy: { hidden: false, searchResult: [] },
    sorensenDiceCoefficient: { hidden: false, searchResult: [] },
    sorensenDiceCoefficientNoisy: { hidden: false, searchResult: [] },
    jaccardIndex: { hidden: false, searchResult: [] },
    jaccardIndexNoisy: { hidden: false, searchResult: [] },
  },
})
export class SearchState {
  @Selector()
  static getIntersectionCountHidden(state: SearchStateModel) {
    return state.intersectionCount.hidden;
  }

  @Selector()
  static getIntersectionCountResult(state: SearchStateModel) {
    return state.intersectionCount.searchResult;
  }

  @Action(SetSourceType)
  setSourceType(ctx: StateContext<SearchStateModel>, { payload }: SetSourceType) {
    // refresh whole model
    ctx.setState({
      sourceType: payload,
      searchedPhrase: '',

      intersectionCount: { hidden: false, searchResult: [] },
      intersectionCountNoisy: { hidden: false, searchResult: [] },
      simpleMatchingCoefficient: { hidden: false, searchResult: [] },
      simpleMatchingCoefficientNoisy: { hidden: false, searchResult: [] },
      sorensenDiceCoefficient: { hidden: false, searchResult: [] },
      sorensenDiceCoefficientNoisy: { hidden: false, searchResult: [] },
      jaccardIndex: { hidden: false, searchResult: [] },
      jaccardIndexNoisy: { hidden: false, searchResult: [] },
    });
  }

  @Action(SetSearchedPhrase)
  setSearchedPhrase(ctx: StateContext<SearchStateModel>, { payload }: SetSearchedPhrase) {
    ctx.setState(
      patch<SearchStateModel>({
        searchedPhrase: payload,
      })
    );

    const state = ctx.getState();
    Object.values(SearchIndexType)
      .filter((indexType: SearchIndexType) => !state[indexType].hidden)
      .forEach((indexType: SearchIndexType) => {
        ctx.dispatch(
          new SendSearchRequest({
            sourceType: state.sourceType,
            searchedPhrase: state.searchedPhrase,
            indexType,
          })
        );
      });
  }
}
