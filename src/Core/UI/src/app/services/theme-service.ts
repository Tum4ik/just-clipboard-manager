import { inject, Service } from '@angular/core';
import { PrimeNG } from 'primeng/config';
import { CommandsService } from './commands-service';

@Service()
export class ThemeService {
  private readonly commandsService = inject(CommandsService);
  private readonly primeNg = inject(PrimeNG);

  readonly themeModes: readonly ThemeMode[] = ['system', 'light', 'dark'];
}


export type ThemeMode = 'system' | 'light' | 'dark';
