import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { SettingsCardComponent } from "../../../settings-card/settings-card.component";

@Component({
  selector: 'jcm-hot-keys-settings',
  templateUrl: './hot-keys-settings.component.html',
  styleUrl: './hot-keys-settings.component.scss',
  imports: [
    ScrollViewComponent,
    SettingsCardComponent,
    TranslatePipe,
  ]
})
export class HotKeysSettingsComponent {

}
