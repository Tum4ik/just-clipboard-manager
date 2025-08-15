import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PluginsService, PluginWithAdditionalInfo } from '@app/core/services/plugins.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { Button } from 'primeng/button';
import { ToggleButton } from 'primeng/togglebutton';
import { Subscription } from 'rxjs';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { ShadedCardComponent } from "../../../shaded-card/shaded-card.component";

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
  ]
})
export class InstalledPlugins implements OnInit, OnDestroy {
  constructor(
    private readonly pluginsService: PluginsService,
    private readonly translateService: TranslateService,
  ) {
    this.lang = this.translateService.currentLang;
  }

  private langChangedSubscription?: Subscription;

  lang: string;

  get plugins(): readonly PluginWithAdditionalInfo[] {
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


  uninstall(plugin: ClipboardDataPlugin) {

  }
}
