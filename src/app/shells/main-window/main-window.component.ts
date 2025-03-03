import { Component } from '@angular/core';
import { Panel } from 'primeng/panel';
import { NavigationBarComponent } from './components/navigation-bar/navigation-bar.component';
import { TitleBarComponent } from './components/title-bar/title-bar.component';

@Component({
  selector: 'jcm-main-window',
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss',
  imports: [
    TitleBarComponent,
    NavigationBarComponent,
    Panel
  ]
})
export class MainWindowComponent {
  constructor(

  ) {
  }


}
