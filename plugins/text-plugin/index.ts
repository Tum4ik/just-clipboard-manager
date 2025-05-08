import packageJson from "./package.json" with { type: "json" };
import { TextPlugin } from "./src/text-plugin.js";

export const pluginInstance = new TextPlugin(packageJson);
