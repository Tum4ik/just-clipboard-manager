import { TestBed } from '@angular/core/testing';

import { GlobalShortcutsService } from './global-shortcuts.service';

describe('GlobalShortcutsService', () => {
  let service: GlobalShortcutsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GlobalShortcutsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
