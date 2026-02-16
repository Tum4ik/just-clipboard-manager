import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ThemePrimaryColor } from './theme-primary-color';

describe('ThemePrimaryColor', () => {
  let component: ThemePrimaryColor;
  let fixture: ComponentFixture<ThemePrimaryColor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ThemePrimaryColor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ThemePrimaryColor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
