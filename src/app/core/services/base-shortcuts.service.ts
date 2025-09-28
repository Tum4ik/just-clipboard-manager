import { MonitoringService } from "@app/core/services/monitoring.service";
import { invoke } from "@tauri-apps/api/core";
import { emit, Event, listen } from "@tauri-apps/api/event";
import { LazyStore } from "@tauri-apps/plugin-store";

const CALL_PASTE_WINDOW = 'call-paste-window';

const CALL_PASTE_WINDOW_SHORTCUT_CHANGED_EVENT_NAME = 'call-paste-window-shortcut-changed-event';

export class BaseShortcutsService {
  constructor(
    protected readonly monitoring: MonitoringService,
  ) {
    this.store = new LazyStore('shortcuts-settings.json', { defaults: {}, autoSave: false });
    listen<ShortcutChangedEvent>(
      CALL_PASTE_WINDOW_SHORTCUT_CHANGED_EVENT_NAME, this.onCallPasteWindowShortcutChanged.bind(this)
    );
  }

  private readonly store: LazyStore;


  async isShortcutRegistered(shortcut: Shortcut): Promise<boolean> {
    try {
      const isReg = await invoke<boolean>('is_shortcut_registered', {
        code: shortcut.code,
        hasCtrl: shortcut.hasCtrl,
        hasShift: shortcut.hasShift,
        hasAlt: shortcut.hasAlt,
        hasMeta: shortcut.hasMeta,
      });
      return isReg;
    }
    catch (err) {
      this.monitoring.error("Global shortcut service:", err);
      return true;
    }
  }


  async getCallPasteWindowShortcutAsync(): Promise<Shortcut> {
    return await this.store.get<Shortcut>(CALL_PASTE_WINDOW) ?? {
      code: 'KeyQ',
      hasCtrl: true,
      hasShift: true,
      hasAlt: false,
      hasMeta: false,
    };
  }

  async setCallPasteWindowShortcutAsync(shortcut: Shortcut): Promise<void> {
    if (!await this.isShortcutRegistered(shortcut)) {
      const oldShortcut = await this.getCallPasteWindowShortcutAsync();
      await this.store.set(CALL_PASTE_WINDOW, shortcut);
      await this.store.save();
      await emit<ShortcutChangedEvent>(CALL_PASTE_WINDOW_SHORTCUT_CHANGED_EVENT_NAME, {
        oldShortcut: oldShortcut, newShortcut: shortcut
      });
    }
  }


  buildShortcutString(shortcut: Shortcut): string {
    const ctrl = shortcut.hasCtrl ? 'Ctrl+' : '';
    const shift = shortcut.hasShift ? 'Shift+' : '';
    const alt = shortcut.hasAlt ? 'Alt+' : '';
    const meta = shortcut.hasMeta ? 'Super+' : '';
    return `${ctrl}${shift}${alt}${meta}${shortcut.code}`;
  }


  protected onCallPasteWindowShortcutChanged(e: Event<ShortcutChangedEvent>) {

  }
}


export interface ShortcutChangedEvent {
  oldShortcut: Shortcut;
  newShortcut: Shortcut;
}


export interface Shortcut {
  code: string;
  hasCtrl: boolean;
  hasShift: boolean;
  hasAlt: boolean;
  hasMeta: boolean;
}
