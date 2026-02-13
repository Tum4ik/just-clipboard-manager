import { TestBed } from '@angular/core/testing';

import { BaseSettingsService } from './base-settings-service';

describe('BaseSettingsService', () => {
  let service: BaseSettingsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BaseSettingsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
