import { Given } from "@wdio/cucumber-framework";
import { MainWindow } from "../page-objects/main-window/main-window";

Given('the Main window is opened',
  async () => await MainWindow.verifyIsOpened()
);
