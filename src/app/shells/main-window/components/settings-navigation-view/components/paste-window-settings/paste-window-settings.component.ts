import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { PasteWindowOpacityService } from '@app/core/services/paste-window-opacity.service';
import { PasteWindowSizingService } from '@app/core/services/paste-window-sizing.service';
import { PasteWindowSnappingService } from '@app/core/services/paste-window-snapping.service';
import { DisplayEdgePosition, SnappingMode } from '@app/core/services/settings.service';
import { TranslatePipe } from '@ngx-translate/core';
import { IftaLabel } from 'primeng/iftalabel';
import { InputNumber, InputNumberInputEvent } from 'primeng/inputnumber';
import { Select } from "primeng/select";
import { Slider } from 'primeng/slider';
import { firstValueFrom } from 'rxjs';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { SettingsCardComponent } from "../../../settings-card/settings-card.component";

@Component({
  selector: 'jcm-paste-window-settings',
  templateUrl: './paste-window-settings.component.html',
  styleUrl: './paste-window-settings.component.scss',
  imports: [
    ScrollViewComponent,
    SettingsCardComponent,
    TranslatePipe,
    Select,
    FormsModule,
    GoogleIcon,
    TranslatePipe,
    IftaLabel,
    InputNumber,
    Slider,
  ]
})
export class PasteWindowSettingsComponent implements OnInit {
  private readonly pasteWindowSnappingService = inject(PasteWindowSnappingService);
  private readonly pasteWindowSizingService = inject(PasteWindowSizingService);
  private readonly pasteWindowOpacityService = inject(PasteWindowOpacityService);


  protected readonly selectedSnappingMode = signal<SnappingMode | null>(null);
  private readonly selectedSnappingModeEffect = effect(async () => {
    const selectedSnappingMode = this.selectedSnappingMode();
    if (selectedSnappingMode) {
      await this.pasteWindowSnappingService.setSnappingModeAsync(selectedSnappingMode);
    }
  });

  protected readonly selectedDisplayEdgePosition = signal<DisplayEdgePosition | null>(null);
  private readonly selectedDisplayEdgePositionEffect = effect(async () => {
    const selectedDisplayEdgePosition = this.selectedDisplayEdgePosition();
    if (selectedDisplayEdgePosition) {
      await this.pasteWindowSnappingService.setDisplayEdgePositionAsync(selectedDisplayEdgePosition);
    }
  });

  // todo: move min/max values to service
  protected readonly minWidth = 200;
  protected readonly maxWidth = 10000;
  protected readonly minHeight = 150;
  protected readonly maxHeight = 1000;

  protected readonly width = toSignal(this.pasteWindowSizingService.width$, { requireSync: true });
  protected readonly height = toSignal(this.pasteWindowSizingService.height$, { requireSync: true });

  protected readonly pinnedClipsHeightPercentage = toSignal(this.pasteWindowSizingService.pinnedClipsHeightPercentage$, { requireSync: true });

  protected readonly opacityPercentage = signal(90);
  private readonly opacityPercentageEffect = effect(async () => {
    const opacityPercentage = this.opacityPercentage();
    await this.pasteWindowOpacityService.setOpacityPercentageAsync(opacityPercentage);
  });


  get snappingModes() {
    return this.pasteWindowSnappingService.snappingModes;
  }
  get displayEdgePositions() {
    return this.pasteWindowSnappingService.displayEdgePositions;
  }

  protected readonly isDisplayEdgesSnappingMode = computed(
    () => this.selectedSnappingMode() === SnappingMode.DisplayEdges
  );


  ngOnInit() {
    this.pasteWindowSnappingService.getSnappingModeAsync().then(mode => this.selectedSnappingMode.set(mode));
    this.pasteWindowSnappingService.getDisplayEdgePositionAsync().then(position => this.selectedDisplayEdgePosition.set(position));
    firstValueFrom(this.pasteWindowOpacityService.opacityPercentage$).then(opacity => this.opacityPercentage.set(opacity));
  }


  protected async setWidth(e: InputNumberInputEvent) {
    await this.pasteWindowSizingService.setSize(e.value as number, this.height());
  }

  protected async setHeight(e: InputNumberInputEvent) {
    await this.pasteWindowSizingService.setSize(this.width(), e.value as number);
  }

  protected async setPinnedClipsHeightPercentage(e: InputNumberInputEvent) {
    await this.pasteWindowSizingService.setPinnedClipsHeightPercentage(e.value as number);
  }


  protected getSnappingModeIconName(mode: SnappingMode): string {
    switch (mode) {
      default:
      case SnappingMode.MouseCursor: return 'arrow_selector_tool';
      case SnappingMode.Caret: return 'text_select_end';
      case SnappingMode.DisplayEdges: return 'picture_in_picture_medium';
    }
  }
}
