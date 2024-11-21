// import { ComponentRef, ViewContainerRef } from '@angular/core';
import { contextBridge, ipcRenderer } from 'electron/renderer';

contextBridge.exposeInMainWorld('electronAPI', {
  // callPlugin: (methodName: string, ...args: any[]) => ipcRenderer.invoke('call-plugin', methodName, ...args),
  callPlugin: (methodName: string) => ipcRenderer.invoke('call-plugin', methodName),
});

contextBridge.exposeInMainWorld('pluginAPI', {
  // getRepresentationDataComponentAsync(viewContainer: ViewContainerRef): Promise<ComponentRef<unknown>> {
  getRepresentationDataComponentAsync(viewContainer: any): Promise<unknown> {
    return ipcRenderer.invoke('getRepresentationDataComponent', viewContainer);
  }
});
