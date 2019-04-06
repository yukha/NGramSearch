import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SearchSourceType } from '../search-source-type.enum';
import { InitData, InitDataService } from '../services/init-data.service';

@Component({
  selector: 'app-search-results',
  templateUrl: './search-results.component.html',
  styleUrls: ['./search-results.component.scss'],
})
export class SearchResultsComponent {
  private searchedPhrase$ = new BehaviorSubject<string>('');

  searchSourceType: SearchSourceType;
  initData: InitData;

  constructor(initDataService: InitDataService, route: ActivatedRoute) {
    this.searchSourceType = route.snapshot.data.searchSourceType;
    this.initData = initDataService.getInitData(this.searchSourceType);

    this.searchedPhrase$
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(
        (phrase: string) => console.log('phrase changed: ', phrase),
        error => console.log(error),
        () => console.log('complete')
      );
  }

  search(event) {
    this.searchedPhrase$.next(event.target.value);
  }
}
