import { Component } from '@angular/core';

import { TitleBarComponent } from './components/title-bar/title-bar.component';

@Component({
  selector: 'jcm-main-window',
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss',
  imports: [
    TitleBarComponent
  ]
})
export class MainWindowComponent {
  constructor(

  ) {
  }


}
