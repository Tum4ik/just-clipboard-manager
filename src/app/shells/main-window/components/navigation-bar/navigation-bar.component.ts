import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { MenuItem } from 'primeng/api';
import { Menu } from 'primeng/menu';
import { Ripple } from 'primeng/ripple';

@Component({
  selector: 'jcm-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrl: './navigation-bar.component.scss',
  imports: [
    Menu,
    Ripple,
    RouterLink,
    RouterLinkActive,
    TranslatePipe
  ]
})
export class NavigationBarComponent {
  navigationItems: MenuItem[] = [
    {
      label: 'settings',
      routerLink: 'settings',
      icon: 'settings',
    },
    {
      label: 'plugins',
      routerLink: 'plugins',
      icon: 'extension',
    },
    {
      label: 'about',
      routerLink: 'about',
      icon: 'info',
    }
  ];
}
