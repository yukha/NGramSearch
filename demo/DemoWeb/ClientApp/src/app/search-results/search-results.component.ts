import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Select, Store } from '@ngxs/store';
import { Observable, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SearchResultLine } from '../models/search-model';
import { ApiServiceService } from '../services/api-service.service';
import { InitData, InitDataService } from '../services/init-data.service';
import { SetSearchedPhrase, SetSourceType } from '../store/search-action';
import { SearchState } from '../store/search-state';

@Component({
  selector: 'app-search-results',
  templateUrl: './search-results.component.html',
  styleUrls: ['./search-results.component.scss'],
})
export class SearchResultsComponent {
  @Select(SearchState.getIntersectionCountHidden) intersectionCountHidden$: Observable<boolean>;
  @Select(SearchState.getIntersectionCountResult) intersectionCountResult$: Observable<SearchResultLine[]>;

  @Select(SearchState.getIntersectionCountNoisyHidden) intersectionCountNoisyHidden$: Observable<boolean>;
  @Select(SearchState.getIntersectionCountNoisyResult) intersectionCountNoisyResult$: Observable<SearchResultLine[]>;

  @Select(SearchState.getSimpleMatchingCoefficientHidden) simpleMatchingCoefficientHidden$: Observable<boolean>;
  @Select(SearchState.getSimpleMatchingCoefficientResult) simpleMatchingCoefficientResult$: Observable<
    SearchResultLine[]
  >;

  @Select(SearchState.getSimpleMatchingCoefficientNoisyHidden) simpleMatchingCoefficientNoisyHidden$: Observable<
    boolean
  >;
  @Select(SearchState.getSimpleMatchingCoefficientNoisyResult) simpleMatchingCoefficientNoisyResult$: Observable<
    SearchResultLine[]
  >;

  @Select(SearchState.getSorensenDiceCoefficientHidden) sorensenDiceCoefficientHidden$: Observable<boolean>;
  @Select(SearchState.getSorensenDiceCoefficientResult) sorensenDiceCoefficientResult$: Observable<SearchResultLine[]>;

  @Select(SearchState.getSorensenDiceCoefficientNoisyHidden) sorensenDiceCoefficientNoisyHidden$: Observable<boolean>;
  @Select(SearchState.getSorensenDiceCoefficientNoisyResult) sorensenDiceCoefficientNoisyResult$: Observable<
    SearchResultLine[]
  >;

  @Select(SearchState.getJaccardIndexHidden) jaccardIndexHidden$: Observable<boolean>;
  @Select(SearchState.getJaccardIndexResult) jaccardIndexResult$: Observable<SearchResultLine[]>;

  @Select(SearchState.getJaccardIndexNoisyHidden) jaccardIndexNoisyHidden$: Observable<boolean>;
  @Select(SearchState.getJaccardIndexNoisyResult) jaccardIndexNoisyResult$: Observable<SearchResultLine[]>;

  private searchedPhrase$ = new Subject<string>();
  initData: InitData;

  constructor(initDataService: InitDataService, route: ActivatedRoute, api: ApiServiceService, private store: Store) {
    const searchSourceType = route.snapshot.data.searchSourceType;
    this.initData = initDataService.getInitData(searchSourceType);

    store.dispatch(new SetSourceType(searchSourceType));

    this.searchedPhrase$
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe((phrase: string) => store.dispatch(new SetSearchedPhrase(phrase)));
  }

  search(event) {
    this.searchedPhrase$.next(event.target.value);
  }
}
