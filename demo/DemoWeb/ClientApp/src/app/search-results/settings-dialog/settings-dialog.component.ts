import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSelectionListChange } from '@angular/material/list';
import { Store } from '@ngxs/store';
import { SearchIndexType } from 'src/app/search-source-type.enum';
import { SetMethodResultVisible } from 'src/app/store/search-action';

@Component({
  selector: 'app-settings-dialog',
  templateUrl: './settings-dialog.component.html',
  styleUrls: ['./settings-dialog.component.scss'],
})
export class SettingsDialogComponent implements OnInit {
  constructor(
    public dialogRef: MatDialogRef<SettingsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SettingsDialogData,
    private store: Store
  ) {}

  ngOnInit() {}

  selectionChange(ev: MatSelectionListChange) {
    this.store.dispatch(
      new SetMethodResultVisible({
        sourceType: ev.option.value,
        searchTypeVisible: ev.option.selected,
      })
    );
  }
}

export interface SettingsDialogData {
  searchTypes: SettingsDialogSearchTypeItem[];
}

export interface SettingsDialogSearchTypeItem {
  searchType: SearchIndexType;
  searchTypeLabel: string;
  searchTypeVisible: boolean;
}
