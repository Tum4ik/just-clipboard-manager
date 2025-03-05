import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Panel } from 'primeng/panel';
import { NotifyingRouterOutlet } from '../../router/notifying-router-outlet';
import { NavigationBarComponent } from './components/navigation-bar/navigation-bar.component';
import { TitleBarComponent } from './components/title-bar/title-bar.component';

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
