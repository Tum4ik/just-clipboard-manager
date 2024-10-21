import { Component, Input, OnInit } from "@angular/core";
import { Router } from "@angular/router";

@Component({
  standalone: true,
  template: ''
})
export class WindowRouterComponent implements OnInit {
  constructor(private readonly router: Router){}

  @Input() window!: string;

  ngOnInit(): void {
    this.router.navigate([this.window])
  }
}
