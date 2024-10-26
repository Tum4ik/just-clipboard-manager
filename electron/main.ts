import { app } from 'electron';
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

app
  .on('window-all-closed', () => { })
  .whenReady().then(async () => {
    const tray = await import("./tray/app-tray.mjs");
    new tray.AppTray(app, __dirname, i18n);
  });
