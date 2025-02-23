import { TestBed } from '@angular/core/testing';

import { PasteWindowVisibilityService } from './paste-window-visibility.service';

describe('PasteWindowVisibilityService', () => {
  let service: PasteWindowVisibilityService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PasteWindowVisibilityService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
