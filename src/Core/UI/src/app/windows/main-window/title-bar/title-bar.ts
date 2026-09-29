import { Component } from '@angular/core';
import { Menubar } from 'primeng/menubar';

@Component({
  selector: 'jcm-title-bar',
  templateUrl: './title-bar.html',
  styleUrl: './title-bar.css',
  imports: [
    Menubar
  ],
})
export class TitleBar { }
