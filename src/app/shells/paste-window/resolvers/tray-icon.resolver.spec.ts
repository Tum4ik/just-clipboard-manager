import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { trayIconResolver } from './tray-icon.resolver';

describe('trayIconResolver', () => {
  const executeResolver: ResolveFn<boolean> = (...resolverParameters) => 
      TestBed.runInInjectionContext(() => trayIconResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
