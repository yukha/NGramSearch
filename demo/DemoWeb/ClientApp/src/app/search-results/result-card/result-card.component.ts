import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-result-card',
  templateUrl: './result-card.component.html',
  styleUrls: ['./result-card.component.scss'],
})
export class ResultCardComponent implements OnInit {
  @Input() title: string;

  @Input() subtitle: string;

  @Input() wikiLink: string;

  constructor() {}

  ngOnInit() {}

  help() {
    console.log('help');
  }
  hide() {
    console.log('hide');
  }
}
