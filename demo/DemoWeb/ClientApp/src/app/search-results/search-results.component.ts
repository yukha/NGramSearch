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
