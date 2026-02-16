import { Component, computed, effect, inject, linkedSignal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { PasteWindowOpacityService } from '@app/core/services/paste-window-opacity.service';
import { PasteWindowSizingService } from '@app/core/services/paste-window-sizing.service';
import { PasteWindowSnappingService } from '@app/core/services/paste-window-snapping.service';
import { SnappingMode } from '@app/core/services/settings.service';
import { TranslatePipe } from '@ngx-translate/core';
import { IftaLabel } from 'primeng/iftalabel';
import { InputNumber, InputNumberInputEvent } from 'primeng/inputnumber';
import { Select } from "primeng/select";
import { Slider } from 'primeng/slider';
import { from } from 'rxjs';
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
export class PasteWindowSettingsComponent {
  private readonly pasteWindowSnappingService = inject(PasteWindowSnappingService);
  private readonly pasteWindowSizingService = inject(PasteWindowSizingService);
  private readonly pasteWindowOpacityService = inject(PasteWindowOpacityService);


  private readonly _selectedSnappingMode = toSignal(from(this.pasteWindowSnappingService.getSnappingModeAsync()));
  protected readonly selectedSnappingMode = linkedSignal(() => this._selectedSnappingMode());
  private readonly selectedSnappingModeEffect = effect(async () => {
    const selectedSnappingMode = this.selectedSnappingMode();
    if (selectedSnappingMode) {
      await this.pasteWindowSnappingService.setSnappingModeAsync(selectedSnappingMode);
    }
  });

  private readonly _selectedDisplayEdgePosition = toSignal(from(this.pasteWindowSnappingService.getDisplayEdgePositionAsync()));
  protected readonly selectedDisplayEdgePosition = linkedSignal(() => this._selectedDisplayEdgePosition());
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

  protected readonly opacityPercentage = toSignal(this.pasteWindowOpacityService.opacityPercentage$);


  protected readonly snappingModes = this.pasteWindowSnappingService.snappingModes;
  protected readonly displayEdgePositions = this.pasteWindowSnappingService.displayEdgePositions;

  protected readonly isDisplayEdgesSnappingMode = computed(
    () => this.selectedSnappingMode() === SnappingMode.DisplayEdges
  );


  protected async setWidth(e: InputNumberInputEvent) {
    const value = e.value;
    if (value){
      await this.pasteWindowSizingService.setSize(value, this.height());
    }
  }

  protected async setHeight(e: InputNumberInputEvent) {
    const value = e.value;
    if (value){
      await this.pasteWindowSizingService.setSize(this.width(), value);
    }
  }

  protected async setPinnedClipsHeightPercentage(e: InputNumberInputEvent) {
    const value = e.value;
    if (value){
      await this.pasteWindowSizingService.setPinnedClipsHeightPercentage(value);
    }
  }

  protected async setOpacityPercentage(value: number | undefined) {
    if (value) {
      await this.pasteWindowOpacityService.setOpacityPercentageAsync(value);
    }
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
