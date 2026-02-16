import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutoDeleteClips } from './auto-delete-clips';

describe('AutoDeleteClips', () => {
  let component: AutoDeleteClips;
  let fixture: ComponentFixture<AutoDeleteClips>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AutoDeleteClips]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AutoDeleteClips);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
