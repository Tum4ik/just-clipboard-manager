import { Given } from "@wdio/cucumber-framework";
import checkTopLevelTabButtonExists from "../support/check/main-window/checkTopLevelTabButtonExists";

Given(
  /^(Settings|Plugins|About) top-level tab button$/,
  checkTopLevelTabButtonExists
);
