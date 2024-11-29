import { app, App } from 'electron';
import electronSquirrelStartup from 'electron-squirrel-startup';
import { Container } from 'inversify';
import { TYPES } from './ioc/types';
import { ClipboardListener } from './services/clipboard-listener';
import { TranslateService } from './services/translate-service';

if (electronSquirrelStartup) {
  app.quit();
}

app
  .on('window-all-closed', () => { })
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

    const container = new Container({ autoBindInjectable: true, defaultScope: 'Transient' });
    container.bind<App>(TYPES.App).toConstantValue(app);
    container.bind<string>(TYPES.AppDir).toConstantValue(__dirname);
    container.bind<TranslateService>(TranslateService).toSelf().inSingletonScope();
    container.bind<ClipboardListener>(ClipboardListener).toSelf().inSingletonScope();
    container.bind<typeof AppTray>(AppTray).toSelf().inSingletonScope();

    await container.get<TranslateService>(TranslateService).initAsync();
    container.get<typeof AppTray>(AppTray);
    container.get<ClipboardListener>(ClipboardListener).start();
  });
