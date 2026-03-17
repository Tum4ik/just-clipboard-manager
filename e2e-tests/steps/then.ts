import { Then } from "@wdio/cucumber-framework";
import { MainWindow } from "../page-objects/main-window/main-window";
import { InstalledView } from "../page-objects/main-window/plugins-view/installed-view/installed-view";

Then('the {string} tab must be selected',
  async (tabName: string) => await MainWindow.verifyTabIsSelected(tabName)
);

Then('the {string} tab content is displayed',
  async (tabName: string) => {
    const view = MainWindow.getTabContentView(tabName);
    await expect(view).toBeDisplayed();
  }
);

Then('the plugin {string} must be installed',
  async (pluginName: string) => {
    const pluginCards = InstalledView.pluginsCards;
    const pluginNames = await pluginCards.map(pc => InstalledView.getPluginCardTitle(pc).getText());
    expect(pluginNames).toContain(pluginName);
  }
);

Then('the plugin {string} must be enabled',
  async (pluginName: string) => {
    const pluginCards = InstalledView.pluginsCards;
    const pluginCard: WebdriverIO.Element = await pluginCards.find(async (card) => {
      const title = await InstalledView.getPluginCardTitle(card).getText();
      return title === pluginName;
    });
    const toggleButton = pluginCard.$('<p-toggle-button />');
    await expect(toggleButton).toHaveAttribute('data-p-checked', 'true');
  }
);
