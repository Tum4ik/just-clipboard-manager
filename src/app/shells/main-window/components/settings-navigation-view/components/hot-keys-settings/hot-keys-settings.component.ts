import { Component, OnInit } from '@angular/core';
import { Shortcut } from '@app/core/services/base-shortcuts.service';
import { GlobalShortcutsSettingService } from '@app/shells/main-window/services/global-shortcuts-setting.service';
import { TranslatePipe } from '@ngx-translate/core';
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
export class HotKeysSettingsComponent implements OnInit {
  constructor(
    private readonly globalShortcutsSettingService: GlobalShortcutsSettingService,
  ) { }

  callPasteWindowShortcut?: Shortcut;

  async ngOnInit() {
    this.callPasteWindowShortcut = await this.globalShortcutsSettingService.getCallPasteWindowShortcutAsync();
  }

  async onCallPasteWindowShortcutChanged(newShortcut: Shortcut) {
    this.callPasteWindowShortcut = newShortcut;
    await this.globalShortcutsSettingService.setCallPasteWindowShortcutAsync(newShortcut);
  }
}
