import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import Database from '@tauri-apps/plugin-sql';

@Injectable({ providedIn: 'root' })
export class EnvironmentService {
  private _isDevelopment = new Promise<boolean>(async resolve => {
    const envStr = await invoke<string>('environment');
    resolve(envStr === 'development');
  });
  isDevelopmentAsync(): Promise<boolean> {
    return this._isDevelopment;
  }

  async isProductionAsync(): Promise<boolean> {
    return !(await this._isDevelopment);
  }

  private _dbConnectionString = new Promise<string>(async resolve => {
    const dbConnectionString = await invoke<string>('db_connection_string');
    try {
      await Database.load(dbConnectionString);
    } catch (error) {
      console.error(error);

      // await invoke<void>('fix_migrations_checksum');
      // await Database.load(dbConnectionString);
    }

    resolve(dbConnectionString);
  });
  getDbConnectionStringAsync(): Promise<string> {
    return this._dbConnectionString;
  }
}
