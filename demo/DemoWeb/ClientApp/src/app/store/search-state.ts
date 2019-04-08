import { Action, Selector, State, StateContext } from '@ngxs/store';
import { patch } from '@ngxs/store/operators';
import { SearchStateModel } from '../models/search-model';
import { SetSearchedPhrase, SetSourceType } from './search-action';

@State<SearchStateModel>({
  name: 'store',
  defaults: {
    sourceType: undefined,
    searchedPhrase: '',

    intersectionCount: { hidden: true, searchResult: [] },
    intersectionCountNoisy: { hidden: true, searchResult: [] },
    simpleMatchingCoefficient: { hidden: true, searchResult: [] },
    simpleMatchingCoefficientNoisy: { hidden: true, searchResult: [] },
    sorensenDiceCoefficient: { hidden: true, searchResult: [] },
    sorensenDiceCoefficientNoisy: { hidden: true, searchResult: [] },
    jaccardIndex: { hidden: true, searchResult: [] },
    jaccardIndexNoisy: { hidden: true, searchResult: [] },
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

      intersectionCount: { hidden: true, searchResult: [] },
      intersectionCountNoisy: { hidden: true, searchResult: [] },
      simpleMatchingCoefficient: { hidden: true, searchResult: [] },
      simpleMatchingCoefficientNoisy: { hidden: true, searchResult: [] },
      sorensenDiceCoefficient: { hidden: true, searchResult: [] },
      sorensenDiceCoefficientNoisy: { hidden: true, searchResult: [] },
      jaccardIndex: { hidden: true, searchResult: [] },
      jaccardIndexNoisy: { hidden: true, searchResult: [] },
    });
  }

  @Action(SetSearchedPhrase)
  setSearchedPhrase(ctx: StateContext<SearchStateModel>, { payload }: SetSearchedPhrase) {
    ctx.setState(
      patch<SearchStateModel>({
        searchedPhrase: payload,
      })
    );
  }
}
