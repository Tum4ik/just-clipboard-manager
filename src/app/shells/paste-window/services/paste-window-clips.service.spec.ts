import { TestBed } from '@angular/core/testing';

import { PasteWindowClipsService } from './paste-window-clips.service';

describe('PasteWindowClipsService', () => {
  let service: PasteWindowClipsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PasteWindowClipsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
