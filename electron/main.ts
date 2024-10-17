import { app, BrowserWindow } from 'electron';
import { AppTray } from './tray/app-tray';

const isServe = process.argv.slice(1).some(arg => arg === '--serve');

const createWindow = () => {
  const win = new BrowserWindow({
    width: 800,
    height: 600
  });

  if (isServe) {
    win.loadURL('http://localhost:4200/');
  }
  else {
    win.loadFile('dist/just-clipboard-manager/browser/index.html');
  }
};

app.whenReady().then(() => {
  createWindow();
  AppTray.initialize(__dirname);
});
