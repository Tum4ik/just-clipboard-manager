import { TestBed } from '@angular/core/testing';

import { PasteDataService } from './paste-data.service';

describe('PasteDataService', () => {
  let service: PasteDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PasteDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
