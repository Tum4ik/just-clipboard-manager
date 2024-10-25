import { app } from 'electron';
import electronSquirrelStartup from 'electron-squirrel-startup';
// import { AppTray } from './tray/app-tray.mjs';


if (electronSquirrelStartup) {
  app.quit();
}


app
  .on('window-all-closed', () => { })
  .whenReady().then(async () => {
    const tray = await import("./tray/app-tray.mjs");
    tray.AppTray.initialize(app, __dirname);
  });
