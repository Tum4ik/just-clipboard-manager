import { Component, OnInit } from '@angular/core';
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
    ScrollViewComponent
  ]
})
export class SearchPluginsComponent implements OnInit {
  constructor(
    private readonly pluginsService: PluginsService
  ) { }

  plugins?: readonly SearchPluginInfo[];

  async ngOnInit(): Promise<void> {
    this.plugins = await this.pluginsService.searchPluginsAsync();
  }
}
