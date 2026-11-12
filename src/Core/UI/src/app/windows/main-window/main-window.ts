import { NgComponentOutlet, NgTemplateOutlet } from '@angular/common';
import { Component, signal, Type } from '@angular/core';
import { TranslateBlockDirective } from '@ngx-translate/core';
import { Ripple } from 'primeng/ripple';
import {
  Sidebar, SidebarAside, SidebarContent, SidebarFooter, SidebarGroup, SidebarGroupContent, SidebarHeader,
  SidebarLayout, SidebarMain, SidebarMenu, SidebarMenuButton, SidebarMenuItem, SidebarPanel, SidebarSpacer
} from 'primeng/sidebar';
import { Tooltip } from 'primeng/tooltip';
import { MaterialIcon } from '../../shared/material-icon/material-icon';
import { GeneralSettings } from './settings/general-settings/general-settings';
import { InterfaceSettings } from './settings/interface-settings/interface-settings';
import { PasteWindowSettings } from './settings/paste-window-settings/paste-window-settings';
import { TitleBar } from './title-bar/title-bar';

@Component({
  selector: 'jcm-main-window',
  templateUrl: './main-window.html',
  styleUrl: './main-window.css',
  imports: [
    TitleBar,
    MaterialIcon,
    SidebarLayout,
    Sidebar,
    SidebarAside,
    SidebarPanel,
    SidebarContent,
    SidebarFooter,
    SidebarGroup,
    SidebarGroupContent,
    SidebarMenu,
    SidebarMenuItem,
    SidebarMenuButton,
    SidebarHeader,
    SidebarSpacer,
    SidebarMain,
    Ripple,
    NgTemplateOutlet,
    NgComponentOutlet,
    TranslateBlockDirective,
    Tooltip,
  ],
})
export class MainWindow {
  // todo: use id for items and sub-items
  protected readonly menuItems: readonly MenuItem[] = [
    {
      title: 'settings._',
      icon: 'settings',
      subMenuItems: [
        {
          title: 'settings.general._',
          icon: 'tune',
          component: GeneralSettings,
        },
        {
          title: 'settings.interface._',
          icon: 'display_settings',
          component: InterfaceSettings,
        },
        {
          title: 'settings.paste-window._',
          icon: 'wysiwyg',
          component: PasteWindowSettings,
        }
      ],
      selectedSubMenuItemIndex: 0
    },
    {
      title: 'plugins._',
      icon: 'extension',
      subMenuItems: [],
      selectedSubMenuItemIndex: 0
    }
  ];

  protected readonly footerMenuItems: readonly MenuItem[] = [
    {
      title: 'about._',
      icon: 'info',
      subMenuItems: []
    }
  ];

  protected selectedMenuItem = signal(this.menuItems[0]);

  protected selectMenuItem(menuItem: MenuItem) {
    this.selectedMenuItem.set(menuItem);
  }

  protected selectSubMenuItem(menuItem: MenuItem, selectedSubMenuItemIndex: number) {
    menuItem.selectedSubMenuItemIndex = selectedSubMenuItemIndex;
  }
}

interface MenuItem {
  title: string;
  icon: string;
  component?: Type<unknown>;
  subMenuItems?: readonly SubMenuItem[];
  selectedSubMenuItemIndex?: number;
}

interface SubMenuItem {
  title: string;
  icon: string;
  component: Type<unknown>;
}
