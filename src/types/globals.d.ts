export { };

declare global {
  interface Window {
    electronAPI: any;
    pluginAPI: any;
  }
}
