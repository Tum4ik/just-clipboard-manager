import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Button } from 'primeng/button';
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
    TranslatePipe
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

  plugins?: readonly SearchPluginInfo[];
  lang: string;

  async ngOnInit(): Promise<void> {
    this.plugins = await this.pluginsService.searchPluginsAsync();
    this.langChangedSubscription = this.translateService.onLangChange.subscribe(e => {
      this.lang = e.lang;
    });
  }

  ngOnDestroy(): void {
    this.langChangedSubscription?.unsubscribe();
  }
}
