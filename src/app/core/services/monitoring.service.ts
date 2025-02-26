import { ErrorHandler, Injectable } from '@angular/core';
import { invoke } from '@tauri-apps/api/core';

@Injectable({ providedIn: 'root' })
export class MonitoringService implements ErrorHandler {
  /**
   * This method is called when unhandled error occurs in the application.
   */
  handleError(error: any): void {
    this.error('Unhandled error', error);
  }

  async info(message: string) {
    console.info(message);
    await invoke('sentry_capture_info', { message });
  }

  async warning(message: string) {
    console.warn(message);
    await invoke('sentry_capture_warning', { message });
  }

  async error(message: string, error?: any) {
    console.error(message, error);
    await invoke('sentry_capture_error', { message: `${message}\n${error?.stack ?? error}` });
  }
}
