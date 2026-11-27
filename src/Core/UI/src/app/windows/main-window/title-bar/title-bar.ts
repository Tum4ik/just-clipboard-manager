import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Menubar } from 'primeng/menubar';
import { from } from 'rxjs';
import { AppCommandsService } from '../../../services/app-commands-service';

@Component({
  selector: 'jcm-title-bar',
  templateUrl: './title-bar.html',
  styleUrl: './title-bar.css',
  imports: [
    Menubar
  ],
})
export class TitleBar {
  private readonly commandsService = inject(AppCommandsService);

  protected readonly productName = toSignal(from(this.commandsService.getProductNameAsync()));
}
