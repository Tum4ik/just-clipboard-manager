import { Component, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Menubar } from 'primeng/menubar';
import { from } from 'rxjs';
import { CommandsService } from '../../../services/commands-service';

@Component({
  selector: 'jcm-title-bar',
  templateUrl: './title-bar.html',
  styleUrl: './title-bar.css',
  imports: [
    Menubar
  ],
})
export class TitleBar {
  constructor(private readonly commandsService: CommandsService) {
    this.productName = toSignal(from(this.commandsService.getProductNameAsync()));
  }

  protected readonly productName: Signal<string | undefined>;
}
