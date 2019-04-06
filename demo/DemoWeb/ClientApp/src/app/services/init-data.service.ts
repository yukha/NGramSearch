import { Injectable } from '@angular/core';
import { SearchSourceType } from '../search-source-type.enum';

@Injectable({
  providedIn: 'root',
})
export class InitDataService {
  getInitData(searchSourceType: SearchSourceType): InitData {
    switch (searchSourceType) {
      case SearchSourceType.actors:
        return {
          title: 'Lists of actors',
          searchLabel: 'Actors',
          searchPlaceholder: 'enter name',
          searchHint: 'Lists of actors',
        };
      case SearchSourceType.films:
        return {
          title: 'Lists of film',
          searchLabel: 'Films',
          searchPlaceholder: 'enter film name',
          searchHint: 'Lists of films',
        };
      default:
        return null;
    }
  }

  constructor() {}
}

export interface InitData {
  title: string;
  searchLabel: string;
  searchPlaceholder: string;
  searchHint: string;
}
