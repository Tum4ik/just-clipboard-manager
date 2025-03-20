import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PasteWindowSettingsComponent } from './paste-window-settings.component';

describe('PasteWindowSettingsComponent', () => {
  let component: PasteWindowSettingsComponent;
  let fixture: ComponentFixture<PasteWindowSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PasteWindowSettingsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PasteWindowSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
