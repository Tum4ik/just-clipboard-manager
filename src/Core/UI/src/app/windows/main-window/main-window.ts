import { NgTemplateOutlet } from '@angular/common';
import { Component } from '@angular/core';
import { Ripple } from 'primeng/ripple';
import { Sidebar, SidebarAside, SidebarContent, SidebarFooter, SidebarGroup, SidebarGroupContent, SidebarHeader, SidebarLayout, SidebarMain, SidebarMenu, SidebarMenuButton, SidebarMenuItem, SidebarPanel, SidebarSpacer } from 'primeng/sidebar';
import { MaterialIcon } from '../../shared/material-icon/material-icon';
import { InterfaceSettings } from './settings/interface-settings/interface-settings';
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
    InterfaceSettings,
    NgTemplateOutlet,
  ],
})
export class MainWindow {

}
