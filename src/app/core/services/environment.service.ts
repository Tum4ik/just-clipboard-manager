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
    await Database.load(dbConnectionString);
    resolve(dbConnectionString);
  });
  getDbConnectionStringAsync(): Promise<string> {
    return this._dbConnectionString;
  }
}
