import { When } from "@wdio/cucumber-framework";
import { MainWindow } from "../page-objects/main-window/main-window";

When('the user clicks the {string} button',
  async (tabName: string) => {
    const tab = MainWindow.getTab(tabName);
    await tab.click();
  }
);
