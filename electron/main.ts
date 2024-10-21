import { app } from 'electron';
import electronSquirrelStartup from 'electron-squirrel-startup';
import { AppTray } from './tray/app-tray';

if (electronSquirrelStartup) {
  app.quit();
}

app
  .on('window-all-closed', () => { })
  .whenReady().then(() => {
    AppTray.initialize(app, __dirname);
  });
