import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsNavigationViewComponent } from './settings-navigation-view.component';

describe('SettingsNavigationViewComponent', () => {
  let component: SettingsNavigationViewComponent;
  let fixture: ComponentFixture<SettingsNavigationViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SettingsNavigationViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SettingsNavigationViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
