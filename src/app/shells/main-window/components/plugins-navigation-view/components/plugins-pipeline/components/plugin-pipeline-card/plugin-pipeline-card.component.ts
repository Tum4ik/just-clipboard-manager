import { Component, input, OnDestroy, OnInit } from '@angular/core';
import { Card } from 'primeng/card';
import { Subscription } from 'rxjs';
import { ThemeService } from '../../../../../../../../core/services/theme.service';

@Component({
  selector: 'jcm-plugin-pipeline-card',
  templateUrl: './plugin-pipeline-card.component.html',
  styleUrl: './plugin-pipeline-card.component.scss',
  imports: [
    Card
  ]
})
export class PluginPipelineCardComponent implements OnInit, OnDestroy {
  constructor(
    private readonly themeService: ThemeService
  ) { }

  private themeChangedSubscription?: Subscription;

  readonly title = input.required<string>();

  surfaceShade = 100;

  ngOnInit(): void {
    this.themeChangedSubscription = this.themeService.themeChanged$.subscribe(themeMode => {
      switch (themeMode) {
        case 'light': this.surfaceShade = 100; break;
        case 'dark': this.surfaceShade = 800; break;
      }
    });
  }

  ngOnDestroy(): void {
    this.themeChangedSubscription?.unsubscribe();
  }
}
