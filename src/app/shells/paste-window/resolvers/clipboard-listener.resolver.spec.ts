import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { clipboardListenerResolver } from './clipboard-listener.resolver';

describe('clipboardListenerResolver', () => {
  const executeResolver: ResolveFn<boolean> = (...resolverParameters) => 
      TestBed.runInInjectionContext(() => clipboardListenerResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
