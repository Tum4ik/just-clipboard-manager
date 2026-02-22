const TOP_LEVEL_TABS_SELECTOR = '[data-testid="top-level-tabs"]';

export abstract class MainWindow {
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

  static async activate(): Promise<void> {
    const maxRetries = 5;
    const retryDelayMs = 500;
    const urls: string[] = [];

    for (let attempt = 0; attempt < maxRetries; attempt++) {
      const handles = await browser.getWindowHandles();

      if (handles.length === 0) {
        urls.push(`[attempt ${attempt + 1}] No window handles found`);
        await browser.pause(retryDelayMs);
        continue;
      }

      for (const handle of handles) {
        try {
          await browser.switchToWindow(handle);
          const url = await browser.getUrl();
          urls.push(url);
          if (url === 'http://tauri.localhost/main-window') {
            return;
          }
        } catch (error) {
          // Handle may have become stale, skip it
          urls.push(`[handle error] ${error instanceof Error ? error.message : String(error)}`);
        }
      }

      // If we haven't found the main window yet, wait and retry
      if (attempt < maxRetries - 1) {
        await browser.pause(retryDelayMs);
      }
    }

    throw new Error(
      `Can't select Main window after ${maxRetries} attempts. ` +
      `Handles checked: ${urls.length}. URLs: ${urls.join(', ')}`
    );
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
