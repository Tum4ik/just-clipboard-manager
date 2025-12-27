import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrimeNgLogo } from './prime-ng-logo';

describe('PrimeNgLogo', () => {
  let component: PrimeNgLogo;
  let fixture: ComponentFixture<PrimeNgLogo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrimeNgLogo]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrimeNgLogo);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
