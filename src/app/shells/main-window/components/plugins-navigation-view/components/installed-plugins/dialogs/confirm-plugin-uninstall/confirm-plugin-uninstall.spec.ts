import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmPluginUninstall } from './confirm-plugin-uninstall';

describe('ConfirmPluginUninstall', () => {
  let component: ConfirmPluginUninstall;
  let fixture: ComponentFixture<ConfirmPluginUninstall>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmPluginUninstall]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfirmPluginUninstall);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
