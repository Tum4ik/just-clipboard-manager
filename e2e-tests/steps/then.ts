import { Then } from "@wdio/cucumber-framework";
import checkTopLevelTabButtonSelected from "../support/check/main-window/checkTopLevelTabButtonSelected";

Then(
  /^(Settings|Plugins|About) tab must be selected$/,
  checkTopLevelTabButtonSelected
);
