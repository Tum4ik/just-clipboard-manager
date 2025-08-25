import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { PasteWindowSnappingService } from '@app/core/services/paste-window-snapping.service';
import { SnappingMode } from '@app/core/services/settings.service';
import { TranslatePipe } from '@ngx-translate/core';
import { Select } from "primeng/select";
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
  ]
})
export class PasteWindowSettingsComponent implements OnInit {
  constructor(
    private readonly pasteWindowSnappingService: PasteWindowSnappingService,
  ) { }

  selectedSnappingMode?: SnappingMode;

  get snappingModes() {
    return this.pasteWindowSnappingService.snappingModes;
  }

  async ngOnInit() {
    this.selectedSnappingMode = await this.pasteWindowSnappingService.getSnappingModeAsync();
  }

  onSnappingModeChanges() {
    if (this.selectedSnappingMode) {
      this.pasteWindowSnappingService.setSnappingModeAsync(this.selectedSnappingMode);
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
}
