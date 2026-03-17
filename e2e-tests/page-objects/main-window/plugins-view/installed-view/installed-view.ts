export abstract class InstalledView {
  private static readonly selfSelector = $('<jcm-installed-plugins />');

  static get pluginsCards(): ChainablePromiseArray {
    return this.selfSelector.$$('<jcm-installed-plugin-card />');
  }

  static getPluginCardTitle(card: ChainablePromiseElement | WebdriverIO.Element): ChainablePromiseElement {
    return card.$('[data-testid="plugin-card-title"]');
  }
}
