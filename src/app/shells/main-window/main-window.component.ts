import { Tab, TabContent, TabList, TabPanel, Tabs } from '@angular/aria/tabs';
import { Component } from '@angular/core';
import { GoogleIcon } from '@app/core/components/google-icon/google-icon';
import { TitleBarComponent } from '@app/core/components/title-bar/title-bar.component';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { Panel } from 'primeng/panel';
import { Ripple } from 'primeng/ripple';
import { AboutViewComponent } from './components/about-view/about-view.component';
import { PluginsNavigationViewComponent } from './components/plugins-navigation-view/plugins-navigation-view.component';
import { SettingsNavigationViewComponent } from './components/settings-navigation-view/settings-navigation-view.component';

@Component({
  selector: 'jcm-main-window',
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss',
  imports: [
    TitleBarComponent,
    SettingsNavigationViewComponent,
    PluginsNavigationViewComponent,
    AboutViewComponent,
    Panel,
    ButtonDirective,

    Ripple,
    TranslatePipe,
    GoogleIcon,

    TabList, Tab, Tabs, TabPanel, TabContent,
  ]
})
export class MainWindowComponent {

}
