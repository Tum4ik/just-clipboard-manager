import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HotKeysSettingsComponent } from './hot-keys-settings.component';

describe('HotKeysSettingsComponent', () => {
  let component: HotKeysSettingsComponent;
  let fixture: ComponentFixture<HotKeysSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HotKeysSettingsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HotKeysSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
