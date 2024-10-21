import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainWindowComponent } from './main-window.component';

describe('MainWindowComponent', () => {
  let component: MainWindowComponent;
  let fixture: ComponentFixture<MainWindowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MainWindowComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MainWindowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
