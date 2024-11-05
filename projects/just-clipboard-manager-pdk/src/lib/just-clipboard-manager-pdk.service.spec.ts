import { TestBed } from '@angular/core/testing';

import { JustClipboardManagerPdkService } from './just-clipboard-manager-pdk.service';

describe('JustClipboardManagerPdkService', () => {
  let service: JustClipboardManagerPdkService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JustClipboardManagerPdkService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
