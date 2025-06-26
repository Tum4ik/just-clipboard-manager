import { Component } from '@angular/core';
import { TitleBarComponent } from '@core/components/title-bar/title-bar.component';
import { Panel } from 'primeng/panel';
import { NotifyingRouterOutlet } from '../../router/notifying-router-outlet';
import { NavigationBarComponent } from './components/navigation-bar/navigation-bar.component';

@Component({
  selector: 'jcm-main-window',
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss',
  imports: [
    TitleBarComponent,
    NavigationBarComponent,
    Panel,
    NotifyingRouterOutlet
  ]
})
export class MainWindowComponent {

}
