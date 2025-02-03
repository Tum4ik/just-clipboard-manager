import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { TranslateService, _ } from "@ngx-translate/core";
import { defaultWindowIcon } from '@tauri-apps/api/app';
import { Image } from "@tauri-apps/api/image";
import { Menu, MenuItem, Submenu, CheckMenuItem } from "@tauri-apps/api/menu";
import { TrayIcon } from '@tauri-apps/api/tray';
import { exit } from '@tauri-apps/plugin-process';

export const trayIconResolver: ResolveFn<void> = async (route, state) => {
  const translate = inject(TranslateService);
  const appName = "Just Clipboard Manager";

  const settingsMenuItem = await MenuItem.new({
    text: 'Settings',
  });
  const aboutMenuItem = await MenuItem.new({
    text: 'About',
  });
  const languageMenuItem = await Submenu.new({
    text: 'Language',
    items: [
      await CheckMenuItem.new({
        text: 'English (United States)',
        action: () => translate.use('en')
      }),
      await CheckMenuItem.new({
        text: 'українська (Україна)',
        action: () => translate.use('uk')
      }),
    ]
  });
  const exitMenuItem = await MenuItem.new({
    text: 'Exit',
    action: () => exit()
  });

  translate.get(_('settings')).subscribe(text => settingsMenuItem.setText(text));
  translate.get(_('about')).subscribe(text => aboutMenuItem.setText(text));
  translate.get(_('language')).subscribe(text => languageMenuItem.setText(text));
  translate.get(_('exit')).subscribe(text => exitMenuItem.setText(text));

  const menu = await Menu.new({
    items: [
      settingsMenuItem,
      aboutMenuItem,
      { kind: 'Predefined', item: 'Separator' },
      languageMenuItem,
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
