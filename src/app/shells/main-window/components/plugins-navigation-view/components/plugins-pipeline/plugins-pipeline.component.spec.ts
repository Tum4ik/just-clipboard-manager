import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PluginsPipelineComponent } from './plugins-pipeline.component';

describe('PluginsPipelineComponent', () => {
  let component: PluginsPipelineComponent;
  let fixture: ComponentFixture<PluginsPipelineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PluginsPipelineComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PluginsPipelineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
