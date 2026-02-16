import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutoStartApplication } from './auto-start-application';

describe('AutoStartApplication', () => {
  let component: AutoStartApplication;
  let fixture: ComponentFixture<AutoStartApplication>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AutoStartApplication]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AutoStartApplication);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
