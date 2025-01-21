import { defaultWindowIcon } from '@tauri-apps/api/app';
import { Image } from "@tauri-apps/api/image";
import { Menu } from "@tauri-apps/api/menu";
import { TrayIcon } from '@tauri-apps/api/tray';
import { exit } from '@tauri-apps/plugin-process';

export async function initializeTrayIconAsync() {
  const appName = "Just Clipboard Manager";
  const menu = await Menu.new({
    items: [
      {
        id: 'exit',
        text: 'Exit',
        action: () => exit()
      },
    ],
  });

  const tray = await TrayIcon.new({
    icon: await defaultWindowIcon() as Image,
    title: appName,
    tooltip: appName,
    menu: menu,
    showMenuOnLeftClick: false,
  });
}
