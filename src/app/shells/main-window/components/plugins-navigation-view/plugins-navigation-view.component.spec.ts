import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PluginsNavigationViewComponent } from './plugins-navigation-view.component';

describe('PluginsNavigationViewComponent', () => {
  let component: PluginsNavigationViewComponent;
  let fixture: ComponentFixture<PluginsNavigationViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PluginsNavigationViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PluginsNavigationViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
