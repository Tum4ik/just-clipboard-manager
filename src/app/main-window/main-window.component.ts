import { Component, ElementRef, Renderer2 } from '@angular/core';

@Component({
  selector: 'jcm-main-window',
  standalone: true,
  imports: [],
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss'
})
export class MainWindowComponent {
  constructor(
    private readonly renderer: Renderer2,
    private readonly element: ElementRef
  ) { }

  async createComp() {


    this.renderer.appendChild(this.element.nativeElement, '');


  }

}
