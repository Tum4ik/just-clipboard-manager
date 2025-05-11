import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Button } from 'primeng/button';
import { ProgressBar } from 'primeng/progressbar';
import { Tag } from 'primeng/tag';
import { Subscription } from 'rxjs';
import { SearchPluginInfo } from '../../../../../../core/data/dto/search-plugin-info.dto';
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
    Tag
  ]
})
export class SearchPluginsComponent implements OnInit, OnDestroy {
  constructor(
    private readonly pluginsService: PluginsService,
    private readonly translateService: TranslateService
  ) {
    this.lang = this.translateService.currentLang;
  }

  private langChangedSubscription?: Subscription;

  plugins?: readonly SearchPluginViewModel[];
  lang: string;

  async ngOnInit(): Promise<void> {
    this.plugins = (await this.pluginsService.searchPluginsAsync()).map(p => {
      // todo: move "isInstalled" detection to plugins service
      const pluginView = p as SearchPluginViewModel;
      pluginView.isInstalled = this.pluginsService.plugins
        .map(installedPlugin => installedPlugin.id)
        .includes(p.id);
      return pluginView;
    });
    this.langChangedSubscription = this.translateService.onLangChange.subscribe(e => {
      this.lang = e.lang;
    });
  }

  ngOnDestroy(): void {
    this.langChangedSubscription?.unsubscribe();
  }


  async install(plugin: SearchPluginViewModel) {
    if (plugin.isInstalling) {
      return;
    }

    plugin.isInstalling = true;

    plugin.isInstalled = await this.pluginsService.installPluginAsync(plugin.downloadLink);
    if (!plugin.isInstalled) {
      // todo: show install error
    }

    plugin.isInstalling = false;
  }
}


export interface SearchPluginViewModel extends SearchPluginInfo {
  isInstalled: boolean;
  isInstalling: boolean;
}
