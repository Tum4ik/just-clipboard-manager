import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HotKeySetting } from './hot-key-setting';

describe('HotKeySetting', () => {
  let component: HotKeySetting;
  let fixture: ComponentFixture<HotKeySetting>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HotKeySetting]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HotKeySetting);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
