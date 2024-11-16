import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'jcm-main-window',
  standalone: true,
  imports: [],
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss'
})
export class MainWindowComponent implements OnInit {
  async ngOnInit() {
    // window.electronAPI.callPlugin('processData').then((r: any) => console.log(r))
    const r = await window.electronAPI.callPlugin('processData');
    console.log(r);
  }

}
