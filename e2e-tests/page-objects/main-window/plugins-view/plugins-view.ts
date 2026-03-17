export abstract class PluginsView {
  private static readonly selfSelector = $('<jcm-plugins-navigation-view />').nextElement();

  static get installedTabButton(): ChainablePromiseElement {
    return this.selfSelector.$('[data-testid="installed"]');
  }
}
