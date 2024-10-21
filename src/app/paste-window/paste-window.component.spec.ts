import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PasteWindowComponent } from './paste-window.component';

describe('PasteWindowComponent', () => {
  let component: PasteWindowComponent;
  let fixture: ComponentFixture<PasteWindowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PasteWindowComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PasteWindowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
