// import { ViewContainerRef } from '@angular/core';
import { spawn } from 'child_process';
import { app, clipboard } from 'electron';
import electronSquirrelStartup from 'electron-squirrel-startup';
import i18n from 'i18next';
import FsBackend, { FsBackendOptions } from 'i18next-fs-backend';
import path from 'path';

if (electronSquirrelStartup) {
  app.quit();
}

i18n.use(FsBackend)
  .init<FsBackendOptions>({
    backend: {
      loadPath: path.join(__dirname, 'i18n/{{lng}}/{{ns}}.yml'),
      addPath: path.join(__dirname, 'i18n/{{lng}}/{{ns}}.missing.yml'),
    },
    initImmediate: false,
    lng: 'en', // todo: use selected lang from settings
    fallbackLng: 'en',
    preload: ['en'], // todo: preload selected lang from settings
    ns: 'translation',
    defaultNS: 'translation'
  });

const clipboardListenerExe = path.join(__dirname, 'dotnet', getExecutable());
const clipboardListenerProcess = spawn(clipboardListenerExe);

app
  .on('will-quit', e => {
    e.preventDefault();
  })
  .on('quit', () => {
    clipboardListenerProcess.kill();
  })
  .whenReady().then(async () => {

    // const pluginUrl = pathToFileURL(`${__dirname}/text-plugin/fesm2022/text-plugin.mjs`, {
    //   windows: process.platform == 'win32'
    // });
    // const pluginModule = await import(pluginUrl.href);
    // const pluginTypeName = Object.keys(pluginModule).at(0)!;
    // const PluginType = pluginModule[pluginTypeName];
    // const pluginInstance/* : ClipboardDataPlugin<unknown> */ = new PluginType();

    // ipcMain.handle('call-plugin', (event, methodName: string) => {
    //   if (typeof pluginInstance[methodName] === 'function') {
    //     const res = pluginInstance[methodName]();
    //     return res;
    //   }
    // });
    // ipcMain.handle('getRepresentationDataComponent', (event, viewContainer) => {
    //   return pluginInstance['getRepresentationDataComponent'](viewContainer);
    // });




    const { AppTray } = await import("./tray/app-tray.mjs");
    new AppTray(app, __dirname, i18n);

    clipboardListenerProcess.stdout.on('data', data => {
      if (data.toString().trim() === 'clipboard-updated') {
        // clipboard updated event
        console.log(clipboard.availableFormats());
      }
    });
  });

function getExecutable(): string {
  switch (process.platform) {
    case 'win32':
      return 'JustClipboardManager.ClipboardListener.exe';
    case 'linux':
      return 'JustClipboardManager.ClipboardListener';
  }

  throw new Error('Unsupported platform');
}
