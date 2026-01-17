import { Component, computed, DOCUMENT, Inject, input, linkedSignal, OnDestroy, output, Renderer2, signal, viewChild } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { Shortcut } from '@app/core/services/base-shortcuts.service';
import { GlobalShortcutsSettingService } from '@app/shells/main-window/services/global-shortcuts-setting.service';
import { isRegistered } from '@tauri-apps/plugin-global-shortcut';
import { Button } from "primeng/button";
import { Tag } from 'primeng/tag';
import { of, switchMap } from 'rxjs';

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

  readonly currentShortcut = input<Shortcut>();
  readonly shortcutChanged = output<Shortcut>();

  readonly shortcutTag = viewChild.required<Tag>('shortcut');

  readonly displayShortcut = linkedSignal(() => this.currentShortcut());

  private readonly isShortcutRegisteredGlobally = toSignal(
    toObservable(this.displayShortcut).pipe(
      switchMap(shortcut => shortcut ? this.globalShortcutsSettingService.isShortcutRegistered(shortcut) : of(false))
    )
  );

  private readonly isShortcutRegisteredByThisApp = toSignal(
    toObservable(this.displayShortcut).pipe(
      switchMap(shortcut => {
        if (!shortcut) {
          return of(false);
        }
        const shortcutString = this.globalShortcutsSettingService.buildShortcutString(shortcut);
        return isRegistered(shortcutString);
      })
    )
  );

  readonly shortcutSeverity = computed(() => {
    if (!this.displayShortcut()) {
      return 'secondary';
    }
    if (this.isShortcutRegisteredGlobally() && !this.isShortcutRegisteredByThisApp()) {
      return 'danger';
    }
    if (this.isEditMode()) {
      return 'success';
    }
    return 'secondary';
  });

  readonly isEditMode = signal(false);

  readonly canAccept = computed(() => {
    return this.displayShortcut() && !this.isShortcutRegisteredGlobally();
  });

  readonly canErase = computed(() => !!this.displayShortcut());

  ngOnDestroy(): void {
    this.document.removeEventListener('keydown', this._keydownListener);
  }

  async edit() {
    this.document.addEventListener('keydown', this._keydownListener);
    this.isEditMode.set(true);
    this.renderer.setStyle(this.shortcutTag().el.nativeElement, 'min-width', `${this.shortcutTag().el.nativeElement.offsetWidth}px`);
    this.displayShortcut.set(undefined);
  }

  accept() {
    if (!this.canAccept()) {
      return;
    }
    this.document.removeEventListener('keydown', this._keydownListener);
    this.isEditMode.set(false);
    this.shortcutChanged.emit(this.displayShortcut()!);
  }

  erase() {
    if (!this.canErase()) {
      return;
    }
    this.displayShortcut.set(undefined);
  }

  cancel() {
    this.document.removeEventListener('keydown', this._keydownListener);
    this.isEditMode.set(false);
    this.displayShortcut.set(this.currentShortcut());
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
