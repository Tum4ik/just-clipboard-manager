import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstalledPlugins } from './installed-plugins';

describe('InstalledPlugins', () => {
  let component: InstalledPlugins;
  let fixture: ComponentFixture<InstalledPlugins>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InstalledPlugins]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InstalledPlugins);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
