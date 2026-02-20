import { When } from "@wdio/cucumber-framework";
import { MainWindow } from "../page-objects/main-window/main-window";

When('the user clicks the {string} tab button',
  async (tabName: string) => {
    const tab = MainWindow.getTabButton(tabName);
    await tab.click();
  }
);
