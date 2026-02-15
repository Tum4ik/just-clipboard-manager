import { Injectable } from "@angular/core";
import { MonitoringService } from "@app/core/services/monitoring.service";
import { MainWindowTabId } from "@app/shells/main-window/main-window.component";
import { invoke } from "@tauri-apps/api/core";
import { Event } from "@tauri-apps/api/event";
import { isRegistered, register, unregister } from "@tauri-apps/plugin-global-shortcut";
import { BaseShortcutsService, Shortcut, ShortcutChangedEvent } from "../../../core/services/base-shortcuts.service";
import { PasteWindowService } from "./paste-window.service";

@Injectable()
export class GlobalShortcutsRegistrationService extends BaseShortcutsService {
  constructor(
    protected override readonly monitoring: MonitoringService,
    private readonly pasteWindowService: PasteWindowService,
  ) {
    super(monitoring);
  }


  async initAsync(): Promise<void> {
    const shortcut = await this.getCallPasteWindowShortcutAsync();
    if (await this.isShortcutRegisteredAsync(shortcut)) {
      await invoke('open_main_window', { topLevelTabId: MainWindowTabId.settings, nestedLevelTabId: 'hot-keys' });
    }
    else {
      await this.registerCallPasteWindowShortcutAsync(shortcut);
    }
  }


  protected override async onCallPasteWindowShortcutChanged(e: Event<ShortcutChangedEvent>) {
    super.onCallPasteWindowShortcutChanged(e);

    const oldShortcutString = this.buildShortcutString(e.payload.oldShortcut);
    try {
      if (await isRegistered(oldShortcutString)) {
        await unregister(oldShortcutString);
      }
      await this.registerCallPasteWindowShortcutAsync(e.payload.newShortcut);
    }
    catch (err) {
      this.monitoring.error('error:', err);
    }
  }


  private async registerCallPasteWindowShortcutAsync(shortcut: Shortcut): Promise<void> {
    const shortcutString = this.buildShortcutString(shortcut);
    await register(shortcutString, async (e) => {
      if (e.state === 'Pressed') {
        await this.pasteWindowService.showAsync();
      }
    });
  }
}
