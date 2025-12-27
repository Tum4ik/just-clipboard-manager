import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyMeACoffee } from './buy-me-a-coffee';

describe('BuyMeACoffee', () => {
  let component: BuyMeACoffee;
  let fixture: ComponentFixture<BuyMeACoffee>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BuyMeACoffee]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BuyMeACoffee);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
