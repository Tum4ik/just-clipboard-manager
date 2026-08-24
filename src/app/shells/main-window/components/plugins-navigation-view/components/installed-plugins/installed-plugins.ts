import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { PluginsService } from '@app/core/services/plugins.service';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { InstalledPluginCard } from './installed-plugin-card/installed-plugin-card';

@Component({
  selector: 'jcm-installed-plugins',
  templateUrl: './installed-plugins.html',
  styleUrl: './installed-plugins.scss',
  imports: [
    ScrollViewComponent,
    InstalledPluginCard,
  ],
})
export class InstalledPlugins {
  private readonly pluginsService = inject(PluginsService);

  protected readonly plugins = toSignal(this.pluginsService.installedPlugins);
}
