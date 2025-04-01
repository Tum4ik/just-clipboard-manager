import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PluginPipelineCardComponent } from './plugin-pipeline-card.component';

describe('PluginPipelineCardComponent', () => {
  let component: PluginPipelineCardComponent;
  let fixture: ComponentFixture<PluginPipelineCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PluginPipelineCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PluginPipelineCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
