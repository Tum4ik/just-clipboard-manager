import { contextBridge, ipcRenderer } from 'electron/renderer';

contextBridge.exposeInMainWorld('electronAPI', {
  // callPlugin: (methodName: string, ...args: any[]) => ipcRenderer.invoke('call-plugin', methodName, ...args),
  callPlugin: (methodName: string) => ipcRenderer.invoke('call-plugin', methodName),
});
