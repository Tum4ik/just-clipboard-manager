import { Component, effect, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SettingsCardComponent } from '@app/shells/main-window/components/settings-card/settings-card.component';
import { TranslatePipe } from '@ngx-translate/core';
import { invoke } from '@tauri-apps/api/core';
import { ToggleSwitch } from 'primeng/toggleswitch';

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
export class AutoStartApplication implements OnInit {
  ngOnInit(): void {
    invoke<boolean>('autostart_is_enabled').then(enabled => this.isAutoStartEnabled.set(enabled));
  }

  protected readonly isAutoStartEnabled = signal(false);
  private readonly isAutoStartEnabledEffect = effect(async () => {
    const checked = this.isAutoStartEnabled();
    if (checked) {
      await invoke('autostart_enable');
    }
    else {
      await invoke('autostart_disable');
    }
  });
}
