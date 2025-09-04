import { Component, OnInit, Signal } from '@angular/core';
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
  constructor(
    private readonly pasteWindowSnappingService: PasteWindowSnappingService,
    private readonly pasteWindowSizingService: PasteWindowSizingService,
    private readonly pasteWindowOpacityService: PasteWindowOpacityService,
  ) {
    this.width = toSignal(this.pasteWindowSizingService.width$, { requireSync: true });
    this.height = toSignal(this.pasteWindowSizingService.height$, { requireSync: true });
    this.pinnedClipsHeightPercentage = toSignal(this.pasteWindowSizingService.pinnedClipsHeightPercentage$, { requireSync: true });
  }

  selectedSnappingMode?: SnappingMode;
  selectedDisplayEdgePosition?: DisplayEdgePosition;

  // todo: move min/max values to service
  readonly minWidth = 200;
  readonly maxWidth = 10000;
  readonly minHeight = 150;
  readonly maxHeight = 1000;
  readonly width: Signal<number>;
  readonly height: Signal<number>;

  readonly pinnedClipsHeightPercentage: Signal<number>;

  opacityPercentage: number = 100;


  get snappingModes() {
    return this.pasteWindowSnappingService.snappingModes;
  }
  get displayEdgePositions() {
    return this.pasteWindowSnappingService.displayEdgePositions;
  }

  get isDisplayEdgesSnappingMode(): boolean {
    return this.selectedSnappingMode === SnappingMode.DisplayEdges;
  }


  async setWidth(e: InputNumberInputEvent) {
    await this.pasteWindowSizingService.setSize(e.value as number, this.height());
  }

  async setHeight(e: InputNumberInputEvent) {
    await this.pasteWindowSizingService.setSize(this.width(), e.value as number);
  }

  async setPinnedClipsHeightPercentage(e: InputNumberInputEvent) {
    await this.pasteWindowSizingService.setPinnedClipsHeightPercentage(e.value as number);
  }


  async ngOnInit() {
    this.selectedSnappingMode = await this.pasteWindowSnappingService.getSnappingModeAsync();
    this.selectedDisplayEdgePosition = await this.pasteWindowSnappingService.getDisplayEdgePositionAsync();
    this.opacityPercentage = await firstValueFrom(this.pasteWindowOpacityService.opacityPercentage$);
  }

  onSnappingModeChanges() {
    if (this.selectedSnappingMode) {
      this.pasteWindowSnappingService.setSnappingModeAsync(this.selectedSnappingMode);
    }
  }

  onDisplayEdgePositionChanges() {
    if (this.selectedDisplayEdgePosition) {
      this.pasteWindowSnappingService.setDisplayEdgePositionAsync(this.selectedDisplayEdgePosition);
    }
  }

  getSnappingModeIconName(mode: SnappingMode): string {
    switch (mode) {
      default:
      case SnappingMode.MouseCursor: return 'arrow_selector_tool';
      case SnappingMode.Caret: return 'text_select_end';
      case SnappingMode.DisplayEdges: return 'picture_in_picture_medium';
    }
  }

  onOpacityChanged() {
    this.pasteWindowOpacityService.setOpacityPercentageAsync(this.opacityPercentage);
  }
}
