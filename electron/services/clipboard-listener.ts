import { ChildProcessWithoutNullStreams, spawn } from 'child_process';
import { type App } from 'electron';
import { inject, injectable } from 'inversify';
import path from 'path';
import { TYPES } from '../ioc/types';
import { ClipboardDataProcessor } from './clipboard-data-processor';

@injectable()
export class ClipboardListener {
  constructor(
    @inject(TYPES.App) private readonly app: App,
    @inject(TYPES.AppDir) private readonly appDir: string,
    private readonly clipboardDataProcessor: ClipboardDataProcessor
  ) { }


  private isStarted = false;
  private clipboardListenerProcess?: ChildProcessWithoutNullStreams;


  start() {
    if (this.isStarted) {
      return;
    }

    const clipboardListenerExe = path.join(this.appDir, 'dotnet', this.getExecutable());
    this.clipboardListenerProcess = spawn(clipboardListenerExe);
    this.clipboardListenerProcess.stdout.on('data', data => {
      if (data.toString().trim() === 'clipboard-updated') {
        this.clipboardDataProcessor.processCurrentItem();
      }
    });

    this.app.on('quit', () => {
      this.stop();
    });

    this.isStarted = true;
  }


  stop() {
    if (!this.isStarted) {
      return;
    }

    this.clipboardListenerProcess?.kill();
    this.isStarted = false;
  }


  private getExecutable(): string {
    switch (process.platform) {
      case 'win32':
        return 'JustClipboardManager.ClipboardListener.exe';
      case 'linux':
        return 'JustClipboardManager.ClipboardListener';
    }

    throw new Error('Unsupported platform');
  }
}
