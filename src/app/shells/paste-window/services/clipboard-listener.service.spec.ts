import { TestBed } from '@angular/core/testing';

import { ClipboardListenerService } from './clipboard-listener.service';

describe('ClipboardListenerService', () => {
  let service: ClipboardListenerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClipboardListenerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
