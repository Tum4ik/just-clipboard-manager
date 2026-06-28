import { inject, Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import Database from '@tauri-apps/plugin-sql';
import { MonitoringService } from './monitoring.service';

@Injectable({ providedIn: 'root' })
export class EnvironmentService {
  private readonly monitoring = inject(MonitoringService);

  private isDevelopment?: boolean;
  async isDevelopmentAsync(): Promise<boolean> {
    if (!this.isDevelopment) {
      this.isDevelopment = await invoke<boolean>('is_development');
    }
    return this.isDevelopment;
  }

  private dbConnectionString?: string;
  async getDbConnectionStringAsync(): Promise<string> {
    if (!this.dbConnectionString) {
      this.dbConnectionString = await invoke<string>('db_connection_string');
      try {
        await Database.load(this.dbConnectionString);
      } catch (error) {
        this.monitoring.error('Failed to load database', error);
        console.error(error);

        // await invoke<void>('fix_migrations_checksum');
        // await Database.load(dbConnectionString);
      }
    }

    return this.dbConnectionString;
  }
}
