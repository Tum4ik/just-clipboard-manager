import { Component, effect, inject, linkedSignal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { GlobalShortcutsSettingService } from '@app/shells/main-window/services/global-shortcuts-setting.service';
import { TranslatePipe } from '@ngx-translate/core';
import { from } from 'rxjs';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { SettingsCardComponent } from "../../../settings-card/settings-card.component";
import { HotKeySetting } from "./hot-key-setting/hot-key-setting";

@Component({
  selector: 'jcm-hot-keys-settings',
  templateUrl: './hot-keys-settings.component.html',
  styleUrl: './hot-keys-settings.component.scss',
  imports: [
    ScrollViewComponent,
    SettingsCardComponent,
    TranslatePipe,
    HotKeySetting,
  ]
})
export class HotKeysSettingsComponent {
  private readonly globalShortcutsSettingService = inject(GlobalShortcutsSettingService);

  private readonly _callPasteWindowShortcut = toSignal(from(this.globalShortcutsSettingService.getCallPasteWindowShortcutAsync()));
  protected readonly callPasteWindowShortcut = linkedSignal(() => this._callPasteWindowShortcut());
  private readonly callPasteWindowShortcutEffect = effect(async () => {
    const newShortcut = this.callPasteWindowShortcut();
    if (newShortcut) {
      await this.globalShortcutsSettingService.setCallPasteWindowShortcutAsync(newShortcut);
    }
  });
}
