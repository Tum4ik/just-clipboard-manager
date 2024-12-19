import { app, App } from 'electron';
import electronSquirrelStartup from 'electron-squirrel-startup';
import { Container } from 'inversify';
import 'reflect-metadata';
import { AppDataSource } from './data/app-data-source';
import { TYPES } from './ioc/types';
import { ClipboardListener } from './services/clipboard-listener';
import { PluginsService } from './services/plugins-service';
import { SettingsService } from './services/settings-service';
import { TranslateService } from './services/translate-service';
import { AppTray } from './tray/app-tray';

if (electronSquirrelStartup) {
  app.quit();
}

const electronAppReady = app
  .on('window-all-closed', () => { })
  .whenReady();
const dataSourceInitialized = AppDataSource.initialize();

Promise.all([electronAppReady, dataSourceInitialized]).then(async () => {
  const container = new Container({ autoBindInjectable: true, defaultScope: 'Transient' });
  container.bind<App>(TYPES.App).toConstantValue(app);
  container.bind<string>(TYPES.AppDir).toConstantValue(__dirname);
  container.bind<SettingsService>(SettingsService).toSelf().inSingletonScope();
  container.bind<TranslateService>(TranslateService).toSelf().inSingletonScope();
  container.bind<ClipboardListener>(ClipboardListener).toSelf().inSingletonScope();
  container.bind<AppTray>(AppTray).toSelf().inSingletonScope();
  container.bind<PluginsService>(PluginsService).toSelf().inSingletonScope();

  await container.get<SettingsService>(SettingsService).initAsync();
  await container.get<TranslateService>(TranslateService).initAsync();
  container.get<AppTray>(AppTray);
  await container.get<PluginsService>(PluginsService).loadPluginsAsync();
  container.get<ClipboardListener>(ClipboardListener).start();
});
