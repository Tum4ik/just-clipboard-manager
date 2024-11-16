import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TextClipComponent } from './text-clip.component';

describe('TextClipComponent', () => {
  let component: TextClipComponent;
  let fixture: ComponentFixture<TextClipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TextClipComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TextClipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
