import { Injectable } from '@angular/core';
import Store from 'electron-store';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private readonly store = new Store();
}
