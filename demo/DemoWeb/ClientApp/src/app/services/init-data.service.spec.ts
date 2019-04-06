import { TestBed } from '@angular/core/testing';

import { InitDataService } from './init-data.service';

describe('InitDataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: InitDataService = TestBed.get(InitDataService);
    expect(service).toBeTruthy();
  });
});
