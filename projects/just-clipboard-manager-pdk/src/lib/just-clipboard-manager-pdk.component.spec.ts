import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JustClipboardManagerPdkComponent } from './just-clipboard-manager-pdk.component';

describe('JustClipboardManagerPdkComponent', () => {
  let component: JustClipboardManagerPdkComponent;
  let fixture: ComponentFixture<JustClipboardManagerPdkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JustClipboardManagerPdkComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JustClipboardManagerPdkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
