import { Component } from '@angular/core';
import { Button } from 'primeng/button';
import { Menubar } from 'primeng/menubar';

@Component({
  selector: 'jcm-main-window',
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss',
  imports: [
    Menubar,
    Button
  ]
})
export class MainWindowComponent {
  constructor(

  ) { }


}
