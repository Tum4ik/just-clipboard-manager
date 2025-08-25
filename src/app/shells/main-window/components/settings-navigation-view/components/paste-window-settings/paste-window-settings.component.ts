import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
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
  ]
})
export class PasteWindowSettingsComponent {

}
