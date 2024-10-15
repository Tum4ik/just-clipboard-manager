import { app, BrowserWindow } from 'electron';

const createWindow = () => {
  const win = new BrowserWindow({
    width: 800,
    height: 600
  });

  win.loadFile('dist/just-clipboard-manager/browser/index.html');
};

app.whenReady().then(() => {
  createWindow();
});
