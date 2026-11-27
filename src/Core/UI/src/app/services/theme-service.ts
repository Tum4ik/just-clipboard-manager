import { inject, Service } from '@angular/core';
import { PrimeNG } from 'primeng/config';
import { AppCommandsService } from './app-commands-service';

@Service()
export class ThemeService {
  private readonly commandsService = inject(AppCommandsService);
  private readonly primeNg = inject(PrimeNG);

  readonly themeModes: readonly ThemeMode[] = ['system', 'light', 'dark'];
}


export type ThemeMode = 'system' | 'light' | 'dark';
