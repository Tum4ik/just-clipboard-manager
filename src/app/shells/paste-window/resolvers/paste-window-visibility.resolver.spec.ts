import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { pasteWindowVisibilityResolver } from './paste-window-visibility.resolver';

describe('pasteWindowVisibilityResolver', () => {
  const executeResolver: ResolveFn<boolean> = (...resolverParameters) => 
      TestBed.runInInjectionContext(() => pasteWindowVisibilityResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
