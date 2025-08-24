import { Component, OnDestroy, OnInit, Signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ClipsRepository } from '@app/core/data/repositories/clips.repository';
import { ExtendedDialogService } from '@app/core/services/extended-dialog.service';
import { PluginsService, PluginWithAdditionalInfo } from '@app/core/services/plugins.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { Button } from 'primeng/button';
import { ToggleButton } from 'primeng/togglebutton';
import { Subscription } from 'rxjs';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { ShadedCardComponent } from "../../../shaded-card/shaded-card.component";
import { ConfirmPluginUninstall, ConfirmPluginUninstallResult } from './dialogs/confirm-plugin-uninstall/confirm-plugin-uninstall';

@Component({
  selector: 'jcm-installed-plugins',
  templateUrl: './installed-plugins.html',
  styleUrl: './installed-plugins.scss',
  imports: [
    ScrollViewComponent,
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
export class InstalledPlugins implements OnInit, OnDestroy {
  constructor(
    private readonly pluginsService: PluginsService,
    private readonly translateService: TranslateService,
    private readonly dialogService: ExtendedDialogService,
  ) {
    this.lang = this.translateService.getCurrentLang();
  }

  private langChangedSubscription?: Subscription;

  lang: string;

  get plugins(): Signal<readonly PluginWithAdditionalInfo[]> {
    return this.pluginsService.installedPlugins;
  }

  ngOnInit(): void {
    this.langChangedSubscription = this.translateService.onLangChange.subscribe(e => {
      this.lang = e.lang;
    });
  }

  ngOnDestroy(): void {
    this.langChangedSubscription?.unsubscribe();
  }


  async uninstall(plugin: ClipboardDataPlugin) {
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
        const repository = new ClipsRepository();
        repository.deleteClipsForPluginAsync(plugin.id);
        return;
    }
  }


  async togglePlugin(plugin: ClipboardDataPlugin, enabled: boolean) {
    if (enabled) {
      await this.pluginsService.enablePluginAsync(plugin.id);
    } else {
      await this.pluginsService.disablePluginAsync(plugin.id);
    }
  }
}
