import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstalledPluginCard } from './installed-plugin-card';

describe('InstalledPluginCard', () => {
  let component: InstalledPluginCard;
  let fixture: ComponentFixture<InstalledPluginCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InstalledPluginCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InstalledPluginCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
