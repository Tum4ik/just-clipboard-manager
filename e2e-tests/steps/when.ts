import { When } from "@wdio/cucumber-framework";
import { MainWindow } from "../page-objects/main-window/main-window";
import { PluginsView } from "../page-objects/main-window/plugins-view/plugins-view";

When('the user clicks the {string} top-level tab button',
  async (tabName: string) => {
    const tab = MainWindow.getTabButton(tabName);
    await tab.click();
  }
);

When('the user clicks the {string} tab button of Plugins view',
  async (tabName: string) => {
    switch (tabName.toLowerCase()) {
      case 'installed':
        await PluginsView.installedTabButton.click();
        break;
    }
  }
);
