import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SearchSourceType } from '../search-source-type.enum';
import { ApiServiceService } from '../services/api-service.service';
import { InitData, InitDataService } from '../services/init-data.service';

@Component({
  selector: 'app-search-results',
  templateUrl: './search-results.component.html',
  styleUrls: ['./search-results.component.scss'],
})
export class SearchResultsComponent {
  private searchedPhrase$ = new BehaviorSubject<string>('');

  searchItems: SearchItem[];
  searchSourceType: SearchSourceType;
  initData: InitData;

  constructor(initDataService: InitDataService, route: ActivatedRoute, api: ApiServiceService) {
    this.searchSourceType = route.snapshot.data.searchSourceType;
    this.initData = initDataService.getInitData(this.searchSourceType);

    const phrase = this.searchedPhrase$.pipe(
      debounceTime(300),
      distinctUntilChanged()
    );

    this.searchItems = [
      {
        searchedPhrase$: phrase,
        searchSourceType: this.searchSourceType,
        api,
      },
    ];

    // .subscribe(
    //   (phrase: string) => console.log('phrase changed: ', phrase),
    //   error => console.log(error),
    //   () => console.log('complete')
    // );
  }

  search(event) {
    this.searchedPhrase$.next(event.target.value);
  }
}

export class SearchItem {
  searchedPhrase$: Observable<string>;
  searchSourceType: SearchSourceType;
  api: ApiServiceService;
}
