import { Component, input, linkedSignal } from '@angular/core';

@Component({
  selector: 'google-icon',
  templateUrl: './google-icon.html',
  styleUrl: './google-icon.scss'
})
export class GoogleIcon {
  readonly iconName = input.required<string>();
  readonly size = input<number>(24);
  readonly isFilled = input<boolean>(false);

  readonly fontVariationSettings = linkedSignal(() => `'FILL' ${this.isFilled() ? 1 : 0}`);
}
