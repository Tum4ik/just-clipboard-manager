import { Component, input } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Menu } from 'primeng/menu';

@Component({
  selector: 'jcm-navigation-view',
  templateUrl: './navigation-view.component.html',
  styleUrl: './navigation-view.component.scss',
  imports: [
    Menu
  ]
})
export class NavigationViewComponent {
  readonly items = input.required<MenuItem[]>();
}
