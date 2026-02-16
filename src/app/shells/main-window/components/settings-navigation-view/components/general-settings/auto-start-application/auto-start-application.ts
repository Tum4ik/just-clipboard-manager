import { Component, effect, linkedSignal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { SettingsCardComponent } from '@app/shells/main-window/components/settings-card/settings-card.component';
import { TranslatePipe } from '@ngx-translate/core';
import { invoke } from '@tauri-apps/api/core';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { from } from 'rxjs';

@Component({
  selector: 'jcm-auto-start-application',
  templateUrl: './auto-start-application.html',
  styleUrl: './auto-start-application.scss',
  imports: [
    ToggleSwitch,
    SettingsCardComponent,
    FormsModule,
    TranslatePipe,
  ],
})
export class AutoStartApplication {
  private readonly _isAutoStartEnabled = toSignal(from(invoke<boolean>('autostart_is_enabled')));
  protected readonly isAutoStartEnabled = linkedSignal(() => this._isAutoStartEnabled());
  private readonly isAutoStartEnabledEffect = effect(async () => {
    const checked = this.isAutoStartEnabled();
    if (checked) {
      await invoke('autostart_enable');
    }
    else if (checked === false) {
      await invoke('autostart_disable');
    }
  });
}
