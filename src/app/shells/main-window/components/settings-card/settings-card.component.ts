import { Component, input, OnDestroy, OnInit } from '@angular/core';
import { Card } from 'primeng/card';
import { Menubar } from 'primeng/menubar';
import { Subscription } from 'rxjs';
import { ThemeService } from '../../../../core/services/theme.service';

@Component({
  selector: 'jcm-settings-card',
  templateUrl: './settings-card.component.html',
  styleUrl: './settings-card.component.scss',
  imports: [
    Card,
    Menubar
  ]
})
export class SettingsCardComponent implements OnInit, OnDestroy {
  constructor(
    private readonly themeService: ThemeService
  ) { }

  private themeChangedSubscription?: Subscription;

  readonly icon = input<string>();
  readonly title = input.required<string>();
  readonly description = input<string>();

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
