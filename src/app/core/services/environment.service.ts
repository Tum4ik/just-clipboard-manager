import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';
import Database from '@tauri-apps/plugin-sql';

@Injectable({ providedIn: 'root' })
export class EnvironmentService {
  private isInitialized = false;

  async initAsync() {
    if (this.isInitialized) {
      return;
    }

    const envStr = await invoke<string>('environment');
    this._isDevelopment = envStr === 'development';
    this._isProduction = envStr === 'production';
    this._dbConnectionString = await invoke<string>('db_connection_string');
    await Database.load(this._dbConnectionString);

    this.isInitialized = true;
  }

  private _isDevelopment = false;
  get isDevelopment(): boolean {
    if (!this.isInitialized) {
      throw new Error('EnvironmentService is not initialized');
    }
    return this._isDevelopment;
  }

  private _isProduction = true;
  get isProduction(): boolean {
    if (!this.isInitialized) {
      throw new Error('EnvironmentService is not initialized');
    }
    return this._isProduction;
  }

  private _dbConnectionString?: string;
  get dbConnectionString(): string {
    if (!this.isInitialized) {
      throw new Error('EnvironmentService is not initialized');
    }
    if (!this._dbConnectionString) {
      throw new Error('DB connection string is not initialized');
    }

    return this._dbConnectionString;
  }
}
