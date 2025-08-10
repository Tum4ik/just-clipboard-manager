import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClipFullDataPreview } from './clip-full-data-preview';

describe('ClipFullDataPreview', () => {
  let component: ClipFullDataPreview;
  let fixture: ComponentFixture<ClipFullDataPreview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClipFullDataPreview]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClipFullDataPreview);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
