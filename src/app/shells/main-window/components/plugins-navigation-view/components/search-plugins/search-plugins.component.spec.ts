import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchPluginsComponent } from './search-plugins.component';

describe('SearchPluginsComponent', () => {
  let component: SearchPluginsComponent;
  let fixture: ComponentFixture<SearchPluginsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchPluginsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SearchPluginsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
