import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { OnRouterAttached } from '../../../../router/notifying-router-outlet';
import { NavigationViewComponent } from '../navigation-view/navigation-view.component';

@Component({
  selector: 'jcm-settings-navigation-view',
  templateUrl: './settings-navigation-view.component.html',
  styleUrl: './settings-navigation-view.component.scss',
  imports: [
    NavigationViewComponent
  ]
})
export class SettingsNavigationViewComponent implements OnRouterAttached {
  constructor(private readonly router: Router) { }

  private activatedHref?: string;

  items: MenuItem[] = [
    {
      label: 'settings.general',
      icon: 'tune',
      routerLink: 'general'
    },
    {
      label: 'settings.interface',
      icon: 'display_settings',
      routerLink: 'interface'
    },
    {
      label: 'settings.paste-window',
      icon: 'wysiwyg',
      routerLink: 'paste-window'
    },
    {
      label: 'settings.hot-keys',
      icon: 'keyboard',
      routerLink: 'hot-keys'
    }
  ];

  onRouterAttached(): void {
    if (this.activatedHref) {
      this.router.navigateByUrl(this.activatedHref);
    }
  }

  onHrefActivated(href: string) {
    this.activatedHref = href;
  }
}
