export abstract class MainWindow {
  static get settingsTabButton() {
    return $('[data-testid="top-level-tabs"] [data-testid="settings"]');
  }

  static get pluginsTabButton() {
    return $('[data-testid="top-level-tabs"] [data-testid="plugins"]');
  }

  static get aboutTabButton() {
    return $('[data-testid="top-level-tabs"] [data-testid="about"]');
  }

  static get topLevelTabContent() {
    return $('[data-testid="top-level-tabs"] [data-testid="top-level-tab-content"]');
  }

  static getTab(tabName: string) {
    switch (tabName) {
      case 'Settings':
        return MainWindow.settingsTabButton;
      case 'Plugins':
        return MainWindow.pluginsTabButton;
      case 'About':
        return MainWindow.aboutTabButton;
      default:
        throw new Error('Undefined Main window tab: ' + tabName);
    }
  }

  static getTabContentView(tabName: string) {
    switch (tabName) {
      case 'Settings':
        return MainWindow.topLevelTabContent.$('<jcm-settings-navigation-view />');
      case 'Plugins':
        return MainWindow.topLevelTabContent.$('<jcm-plugins-navigation-view />');
      case 'About':
        return MainWindow.topLevelTabContent.$('<jcm-about-view />');
      default:
        throw new Error('Undefined Main window tab: ' + tabName);
    }
  }

  static async verifyIsOpened(): Promise<void> {
    const url = await browser.getUrl();
    expect(url.endsWith('/main-window')).toBeTruthy();
  }

  static async verifyTabIsSelected(tabName: string): Promise<void> {
    const tab = this.getTab(tabName);
    await expect(tab).toHaveAttribute('aria-selected', 'true');
  }
}
