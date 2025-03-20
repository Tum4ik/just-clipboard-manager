import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { globalShortcutsResolver } from './global-shortcuts.resolver';

describe('globalShortcutsResolver', () => {
  const executeResolver: ResolveFn<boolean> = (...resolverParameters) => 
      TestBed.runInInjectionContext(() => globalShortcutsResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
