import { Then } from "@wdio/cucumber-framework";
import { MainWindow } from "../page-objects/main-window/main-window";

Then('the {string} tab must be selected',
  async (tabName: string) => await MainWindow.verifyTabIsSelected(tabName)
);

Then('the {string} tab content is displayed',
  async (tabName: string) => {
    const view = MainWindow.getTabContentView(tabName);
    await expect(view).toBeDisplayed();
  }
);
