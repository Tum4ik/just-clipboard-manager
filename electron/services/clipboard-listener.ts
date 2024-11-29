import { ChildProcessWithoutNullStreams, spawn } from 'child_process';
import { type App, clipboard } from 'electron';
import { inject, injectable } from 'inversify';
import path from 'path';
import 'reflect-metadata';
import { TYPES } from '../ioc/types';

@injectable()
export class ClipboardListener {
  constructor(
    @inject(TYPES.App) private readonly app: App,
    @inject(TYPES.AppDir) private readonly appDir: string,
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
        // clipboard updated event
        console.log(clipboard.availableFormats());
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
