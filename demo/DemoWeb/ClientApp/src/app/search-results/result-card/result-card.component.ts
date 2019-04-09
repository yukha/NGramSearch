import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngxs/store';
import { SearchResultLine } from 'src/app/models/search-model';

@Component({
  selector: 'app-result-card',
  templateUrl: './result-card.component.html',
  styleUrls: ['./result-card.component.scss'],
})
export class ResultCardComponent implements OnInit {
  @Input() title: string;

  @Input() subtitle: string;

  @Input() wikiLink: string;

  @Input() results: SearchResultLine[] = [];

  // private resultsField: SearchResultLine[];

  // @Input() set results(value: SearchResultLine[]) {
  //   console.log(value);
  //   this.resultsField = value;
  // }
  // get results(): SearchResultLine[] {
  //   return !this.resultsField ? [] : this.resultsField;
  // }

  constructor(private store: Store) {}

  ngOnInit() {}

  help() {
    console.log('help');
  }
  hide() {
    console.log('hide');
  }
}
