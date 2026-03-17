import { Component, computed, effect, inject, input, linkedSignal, untracked } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ClipsRepository } from '@app/core/data/repositories/clips.repository';
import { ExtendedDialogService } from '@app/core/services/extended-dialog.service';
import { PluginsService, PluginWithAdditionalInfo } from '@app/core/services/plugins.service';
import { ShadedCardComponent } from '@app/shells/main-window/components/shaded-card/shaded-card.component';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ClipboardDataPlugin, PluginId } from 'just-clipboard-manager-pdk';
import { Button } from 'primeng/button';
import { ToggleButton } from 'primeng/togglebutton';
import { ConfirmPluginUninstall, ConfirmPluginUninstallResult } from '../dialogs/confirm-plugin-uninstall/confirm-plugin-uninstall';

@Component({
  selector: 'jcm-installed-plugin-card',
  templateUrl: './installed-plugin-card.html',
  styleUrl: './installed-plugin-card.scss',
  imports: [
    ShadedCardComponent,
    TranslatePipe,
    Button,
    ToggleButton,
    FormsModule,
  ],
  providers: [
    ExtendedDialogService
  ]
})
export class InstalledPluginCard {
  private readonly pluginsService = inject(PluginsService);
  private readonly translateService = inject(TranslateService);
  private readonly dialogService = inject(ExtendedDialogService);
  private readonly clipsRepository = inject(ClipsRepository);


  readonly pluginInfo = input.required<PluginWithAdditionalInfo>();


  private readonly langChangeEvent = toSignal(this.translateService.onLangChange);
  protected readonly lang = computed(() => this.langChangeEvent()?.lang ?? this.translateService.getCurrentLang());

  protected readonly isEnabled = linkedSignal(() => this.pluginInfo().isEnabled);
  private readonly isEnabledEffect = effect(async () => {
    const pluginInfo = untracked(() => this.pluginInfo());
    if (this.isEnabled() === pluginInfo.isEnabled) {
      console.log("no save");
      return;
    }
    const pluginId = pluginInfo.plugin.id;
    if (this.isEnabled()) {
      await this.pluginsService.enablePluginAsync(pluginId);
    } else {
      await this.pluginsService.disablePluginAsync(pluginId);
    }
  });


  protected async uninstall(plugin: ClipboardDataPlugin) {
    const result = await this.dialogService.openAsync(ConfirmPluginUninstall, {
      header: `${this.translateService.instant('uninstall')} ${plugin.name}`
    });
    switch (result) {
      case ConfirmPluginUninstallResult.Cancel:
        return;
      case ConfirmPluginUninstallResult.RemoveOnlyPlugin:
        this.pluginsService.uninstallPluginAsync(plugin.id);
        return;
      case ConfirmPluginUninstallResult.RemovePluginAndClips:
        this.pluginsService.uninstallPluginAsync(plugin.id);
        this.clipsRepository.deleteClipsForPluginAsync(plugin.id);
        return;
    }
  }


  protected isBuiltInPlugin(pluginId: PluginId) {
    return this.pluginsService.isBuiltInPlugin(pluginId);
  }
}
