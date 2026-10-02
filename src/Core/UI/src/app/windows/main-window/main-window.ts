import { Component } from '@angular/core';
import { TitleBar } from './title-bar/title-bar';

@Component({
  selector: 'jcm-main-window',
  templateUrl: './main-window.html',
  styleUrl: './main-window.css',
  imports: [
    TitleBar
  ],
})
export class MainWindow {

}
