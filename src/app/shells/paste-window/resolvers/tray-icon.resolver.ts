import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { TranslateService, _ } from "@ngx-translate/core";
import { defaultWindowIcon } from '@tauri-apps/api/app';
import { Image } from "@tauri-apps/api/image";
import { Menu, MenuItem } from "@tauri-apps/api/menu";
import { TrayIcon } from '@tauri-apps/api/tray';
import { exit } from '@tauri-apps/plugin-process';

export const trayIconResolver: ResolveFn<void> = async (route, state) => {
  const translate = inject(TranslateService);
  const appName = "Just Clipboard Manager";

  const settingsMenuItem = await MenuItem.new({
    id: 'settings',
    text: '',
  });
  const aboutMenuItem = await MenuItem.new({
    id: 'about',
    text: '',
  });
  const exitMenuItem = await MenuItem.new({
    id: 'exit',
    text: '',
    action: () => exit()
  });

  translate.get(_('tray.settings')).subscribe(text => settingsMenuItem.setText(text));
  translate.get(_('tray.about')).subscribe(text => aboutMenuItem.setText(text));
  translate.get(_('tray.exit')).subscribe(text => exitMenuItem.setText(text));

  const menu = await Menu.new({
    items: [
      settingsMenuItem,
      aboutMenuItem,
      { kind: 'Predefined', item: 'Separator' },
      exitMenuItem,
    ],
  });

  const tray = await TrayIcon.new({
    icon: await defaultWindowIcon() as Image,
    title: appName,
    tooltip: appName,
    menu: menu,
    showMenuOnLeftClick: false,
  });
};
