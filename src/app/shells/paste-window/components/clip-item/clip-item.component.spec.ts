import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClipItemComponent } from './clip-item.component';

describe('ClipItemComponent', () => {
  let component: ClipItemComponent;
  let fixture: ComponentFixture<ClipItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClipItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClipItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
