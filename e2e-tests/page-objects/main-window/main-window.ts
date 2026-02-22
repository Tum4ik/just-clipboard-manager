import { BaseWindow } from "../base-window";

const TOP_LEVEL_TABS_SELECTOR = '[data-testid="top-level-tabs"]';

export abstract class MainWindow extends BaseWindow {
  static async activate(): Promise<void> {
    await this.baseActivate('http://tauri.localhost/main-window');
  }


  static get settingsTabButton() {
    return $(`${TOP_LEVEL_TABS_SELECTOR} [data-testid="settings"]`);
  }

  static get pluginsTabButton() {
    return $(`${TOP_LEVEL_TABS_SELECTOR} [data-testid="plugins"]`);
  }

  static get aboutTabButton() {
    return $(`${TOP_LEVEL_TABS_SELECTOR} [data-testid="about"]`);
  }

  static get topLevelTabContent() {
    return $(`${TOP_LEVEL_TABS_SELECTOR} [data-testid="top-level-tab-content"]`);
  }

  static getTabButton(tabName: string): ChainablePromiseElement {
    const tab = this.getTab(tabName);
    return tab.tabButton;
  }

  static getTabContentView(tabName: string): ChainablePromiseElement {
    const tab = this.getTab(tabName);
    return tab.tabContentView;
  }

  static async verifyTabIsSelected(tabName: string): Promise<void> {
    const tab = this.getTabButton(tabName);
    await expect(tab).toHaveAttribute('aria-selected', 'true');
  }


  private static getTab(tabName: string): Tab {
    const tab = this.tabs[tabName];
    if (!tab) {
      throw new Error('Undefined Main window tab: ' + tabName);
    }
    return tab;
  }

  private static readonly tabs: Record<string, Tab> = {
    'Settings': {
      tabButton: this.settingsTabButton,
      tabContentView: this.topLevelTabContent.$('<jcm-settings-navigation-view />'),
    },
    'Plugins': {
      tabButton: this.pluginsTabButton,
      tabContentView: this.topLevelTabContent.$('<jcm-plugins-navigation-view />'),
    },
    'About': {
      tabButton: this.aboutTabButton,
      tabContentView: this.topLevelTabContent.$('<jcm-about-view />'),
    }
  };
}

type Tab = { tabButton: ChainablePromiseElement; tabContentView: ChainablePromiseElement; };
