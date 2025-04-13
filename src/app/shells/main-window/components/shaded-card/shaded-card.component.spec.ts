import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShadedCardComponent } from './shaded-card.component';

describe('ShadedCardComponent', () => {
  let component: ShadedCardComponent;
  let fixture: ComponentFixture<ShadedCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShadedCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShadedCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
