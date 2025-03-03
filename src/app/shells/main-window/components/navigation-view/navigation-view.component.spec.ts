import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavigationViewComponent } from './navigation-view.component';

describe('NavigationViewComponent', () => {
  let component: NavigationViewComponent;
  let fixture: ComponentFixture<NavigationViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NavigationViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NavigationViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
