import { Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';

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
}
