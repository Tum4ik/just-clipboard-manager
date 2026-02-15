import { Component, computed, DOCUMENT, effect, Inject, linkedSignal, model, OnDestroy, Renderer2, signal, viewChild } from '@angular/core';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { Shortcut } from '@app/core/services/base-shortcuts.service';
import { GlobalShortcutsSettingService } from '@app/shells/main-window/services/global-shortcuts-setting.service';
import { isRegistered } from '@tauri-apps/plugin-global-shortcut';
import { Button } from "primeng/button";
import { Tag } from 'primeng/tag';

@Component({
  selector: 'jcm-hot-key-setting',
  templateUrl: './hot-key-setting.html',
  styleUrl: './hot-key-setting.scss',
  imports: [
    Tag,
    Button,
    GoogleIcon
  ]
})
export class HotKeySetting implements OnDestroy {
  constructor(
    @Inject(DOCUMENT) private readonly document: Document,
    private readonly globalShortcutsSettingService: GlobalShortcutsSettingService,
    private readonly renderer: Renderer2
  ) { }

  // readonly currentShortcut = input<Shortcut>();
  // readonly shortcutChanged = output<Shortcut>();
  readonly shortcut = model<Shortcut>();

  private readonly shortcutTag = viewChild.required<Tag>('shortcut');

  // protected readonly displayShortcut = linkedSignal(() => this.currentShortcut());
  protected readonly displayShortcut = linkedSignal(() => this.shortcut());
  private readonly effect = effect(async () => {
    const displayShortcut = this.displayShortcut();
    const isShortcutRegisteredGlobally = displayShortcut
      ? await this.globalShortcutsSettingService.isShortcutRegisteredAsync(displayShortcut)
      : false;
    this.isShortcutRegisteredGlobally.set(isShortcutRegisteredGlobally);

    let isShortcutRegisteredByThisApp = false;
    if (displayShortcut) {
      const shortcutString = this.globalShortcutsSettingService.buildShortcutString(displayShortcut);
      isShortcutRegisteredByThisApp = await isRegistered(shortcutString);
    }
    this.isShortcutRegisteredByThisApp.set(isShortcutRegisteredByThisApp);
  });

  private readonly isShortcutRegisteredGlobally = signal(false);
  private readonly isShortcutRegisteredByThisApp = signal(false);

  protected readonly shortcutSeverity = computed(() => {
    const displayShortcut = this.displayShortcut();
    const isShortcutRegisteredGlobally = this.isShortcutRegisteredGlobally();
    const isShortcutRegisteredByThisApp = this.isShortcutRegisteredByThisApp();
    const isEditMode = this.isEditMode();
    if (!displayShortcut) {
      return 'secondary';
    }
    if (isShortcutRegisteredGlobally && !isShortcutRegisteredByThisApp) {
      return 'danger';
    }
    if (isEditMode) {
      return 'success';
    }
    return 'secondary';
  });

  protected readonly isEditMode = signal(false);

  protected readonly canAccept = computed(() => {
    const displayShortcut = this.displayShortcut();
    const isShortcutRegisteredGlobally = this.isShortcutRegisteredGlobally();
    return displayShortcut && !isShortcutRegisteredGlobally;
  });

  protected readonly canErase = computed(() => !!this.displayShortcut());

  ngOnDestroy(): void {
    this.document.removeEventListener('keydown', this._keydownListener);
  }

  protected async edit() {
    this.document.addEventListener('keydown', this._keydownListener);
    this.isEditMode.set(true);
    this.renderer.setStyle(this.shortcutTag().el.nativeElement, 'min-width', `${this.shortcutTag().el.nativeElement.offsetWidth}px`);
    this.displayShortcut.set(undefined);
  }

  protected accept() {
    if (!this.canAccept()) {
      return;
    }
    this.document.removeEventListener('keydown', this._keydownListener);
    this.isEditMode.set(false);
    // this.shortcutChanged.emit(this.displayShortcut()!);
    this.shortcut.set(this.displayShortcut());
  }

  protected erase() {
    if (!this.canErase()) {
      return;
    }
    this.displayShortcut.set(undefined);
  }

  protected cancel() {
    this.document.removeEventListener('keydown', this._keydownListener);
    this.isEditMode.set(false);
    // this.displayShortcut.set(this.currentShortcut());
    this.displayShortcut.set(this.shortcut());
  }


  private readonly _keydownListener = this.keydownListener.bind(this);
  private async keydownListener(e: KeyboardEvent) {
    e.preventDefault();
    if (!this.hasModifiers(e) || this.isModifier(e)) {
      return;
    }

    const shortcut: Shortcut = {
      code: e.code,
      hasCtrl: e.ctrlKey,
      hasShift: e.shiftKey,
      hasAlt: e.altKey,
      hasMeta: e.metaKey,
    };

    this.displayShortcut.set(shortcut);
  }


  private hasModifiers(e: KeyboardEvent): boolean {
    return e.ctrlKey || e.altKey || e.metaKey; // shift must be skipped here
  }

  private isModifier(e: KeyboardEvent): boolean {
    return e.code === 'ControlLeft' || e.code === 'ControlRight'
      || e.code === 'AltLeft' || e.code === 'AltRight'
      || e.code === 'ShiftLeft' || e.code === 'ShiftRight'
      || e.code === 'MetaLeft' || e.code === 'MetaRight';
  }
}
