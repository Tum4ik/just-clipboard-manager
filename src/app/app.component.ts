import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  imports: [RouterOutlet]
})
export class AppComponent implements OnInit {
  constructor(private readonly themeService: ThemeService) { }

  ngOnInit(): void {
    this.themeService.initAsync();
  }
}
