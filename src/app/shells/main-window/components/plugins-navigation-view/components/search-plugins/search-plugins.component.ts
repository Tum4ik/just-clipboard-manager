import { AsyncPipe } from '@angular/common';
import { Component, computed, Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Button } from 'primeng/button';
import { ProgressBar } from 'primeng/progressbar';
import { Skeleton } from 'primeng/skeleton';
import { Tag } from 'primeng/tag';
import { map } from 'rxjs';
import { SearchPluginInfo } from '../../../../../../core/dto/search-plugin-info.dto';
import { PluginsService } from '../../../../../../core/services/plugins.service';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { ShadedCardComponent } from "../../../shaded-card/shaded-card.component";

@Component({
  selector: 'jcm-search-plugins',
  templateUrl: './search-plugins.component.html',
  styleUrl: './search-plugins.component.scss',
  imports: [
    ShadedCardComponent,
    ScrollViewComponent,
    Button,
    TranslatePipe,
    ProgressBar,
    Tag,
    Skeleton,
    GoogleIcon,
    AsyncPipe,
  ]
})
export class SearchPluginsComponent {
  constructor(
    private readonly pluginsService: PluginsService,
    private readonly translateService: TranslateService
  ) {
    this.lang = toSignal(
      this.translateService.onLangChange.pipe(map(e => e.lang)),
      { initialValue: translateService.currentLang }
    );
  }

  private searchPlugins?: Promise<SearchPluginInfo[]>;
  readonly plugins = computed<Promise<readonly SearchPluginViewModel[]>>(() => {
    const installedPlugins = this.pluginsService.installedPlugins();
    this.searchPlugins ??= this.pluginsService.searchPluginsAsync();
    return this.searchPlugins.then(plugins => {
      return plugins.map(p => {
        const pluginView = p as SearchPluginViewModel;
        pluginView.isInstalled = installedPlugins
          .map(installedPlugin => installedPlugin.plugin.id)
          .includes(p.id);
        return pluginView;
      });
    });
  });
  readonly lang: Signal<string>;


  async install(plugin: SearchPluginViewModel) {
    if (plugin.isInstalling) {
      return;
    }

    plugin.isInstalling = true;

    plugin.isInstalled = await this.pluginsService.installPluginAsync(plugin.id, plugin.downloadLink);
    if (!plugin.isInstalled) {
      plugin.isInstallationFailed = true;
    }

    plugin.isInstalling = false;
  }
}


export interface SearchPluginViewModel extends SearchPluginInfo {
  isInstalling: boolean;
  isInstalled: boolean;
  isInstallationFailed: boolean;
}
