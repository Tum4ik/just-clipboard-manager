import { NgTemplateOutlet } from '@angular/common';
import { Component, contentChild, input, OnDestroy, OnInit, TemplateRef } from '@angular/core';
import { Card } from 'primeng/card';
import { Subscription } from 'rxjs';
import { ThemeService } from '../../../../core/services/theme.service';

@Component({
  selector: 'jcm-shaded-card',
  templateUrl: './shaded-card.component.html',
  styleUrl: './shaded-card.component.scss',
  imports: [
    Card,
    NgTemplateOutlet
  ]
})
export class ShadedCardComponent implements OnInit, OnDestroy {
  constructor(
    private readonly themeService: ThemeService,
  ) { }

  private themeChangedSubscription?: Subscription;

  private readonly shades: Map<Level, { light: number, dark: number; }> = new Map([
    [0, { light: 50, dark: 900 }],
    [1, { light: 100, dark: 800 }],
    [2, { light: 200, dark: 700 }],
    [3, { light: 300, dark: 600 }],
    [4, { light: 400, dark: 500 }],
    [5, { light: 500, dark: 400 }],
    [6, { light: 600, dark: 300 }],
    [7, { light: 700, dark: 200 }],
    [8, { light: 800, dark: 100 }],
    [9, { light: 900, dark: 50 }],
  ]);

  readonly titleTemplate = contentChild('title', { read: TemplateRef });
  readonly subtitleTemplate = contentChild('subtitle', { read: TemplateRef });
  readonly footerTemplate = contentChild('footer', { read: TemplateRef });

  readonly shadeLevel = input<Level>(1);
  readonly cardBodyPadding = input<string>('12px');

  surfaceShade = 100;

  ngOnInit(): void {
    this.themeChangedSubscription = this.themeService.isDarkTheme$.subscribe(isDarkTheme => {
      if (isDarkTheme) {
        this.surfaceShade = this.shades.get(this.shadeLevel())!.dark;
      }
      else {
        this.surfaceShade = this.shades.get(this.shadeLevel())!.light;
      }
    });
  }

  ngOnDestroy(): void {
    this.themeChangedSubscription?.unsubscribe();
  }
}

type Level = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9;
