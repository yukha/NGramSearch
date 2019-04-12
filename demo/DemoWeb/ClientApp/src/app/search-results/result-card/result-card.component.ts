import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { SearchResultLine } from 'src/app/models/search-model';

@Component({
  selector: 'app-result-card',
  templateUrl: './result-card.component.html',
  styleUrls: ['./result-card.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResultCardComponent {
  @Input() title: string;

  @Input() subtitle: string;

  @Input() wikiLink: string;

  @Input() results: SearchResultLine[] = [];

  displayedColumns: string[] = ['similarity', 'result'];

  help() {
    console.log('help');
  }
  hide() {
    console.log('hide');
  }
}
