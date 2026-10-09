import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'jcm-material-icon',
  templateUrl: './material-icon.html',
  styleUrl: './material-icon.css',
})
export class MaterialIcon {
  readonly iconName = input.required<string>();
  readonly size = input<number>(24);
  readonly isFilled = input<boolean>(false);

  protected readonly fontVariationSettings = computed(() => `'FILL' ${this.isFilled() ? 1 : 0}`);
}
