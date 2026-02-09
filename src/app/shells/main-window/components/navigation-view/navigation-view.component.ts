import { Tab, TabContent, TabList, TabPanel, Tabs } from '@angular/aria/tabs';
import { NgComponentOutlet } from '@angular/common';
import { Component, computed, input } from '@angular/core';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { Ripple } from 'primeng/ripple';
import { NavigationMenuItem } from './navigation-view';

@Component({
  selector: 'jcm-navigation-view',
  templateUrl: './navigation-view.component.html',
  styleUrl: './navigation-view.component.scss',
  imports: [
    Ripple,
    ButtonDirective,
    NgComponentOutlet,
    TranslatePipe,
    GoogleIcon,
    TabList, Tab, Tabs, TabPanel, TabContent,
  ]
})
export class NavigationViewComponent {
  readonly items = input.required<NavigationMenuItem[]>();
  readonly nestedLevelTabId = input<string>();

  protected readonly selectedItem = computed(() => {
    const tabId = this.nestedLevelTabId();
    const itemsIds = this.items().map(i => i.id);
    if (tabId && itemsIds.includes(tabId)) {
      return tabId;
    }
    return itemsIds[0];
  });
}
