import { Component, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { MenuItem } from 'primeng/api';
import { Menu } from 'primeng/menu';
import { Ripple } from 'primeng/ripple';

@Component({
  selector: 'jcm-navigation-view',
  templateUrl: './navigation-view.component.html',
  styleUrl: './navigation-view.component.scss',
  imports: [
    Menu,
    Ripple,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    TranslatePipe
  ]
})
export class NavigationViewComponent {
  readonly items = input.required<MenuItem[]>();
  readonly hrefActivated = output<string>();

  navigationItemClicked(url: string) {
    // todo: cache "url -> href"
    const href = new URL(url).pathname;
    this.hrefActivated.emit(href);
  }
}
