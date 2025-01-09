import { contextBridge, ipcRenderer } from 'electron/renderer';

contextBridge.exposeInMainWorld('clipsServiceAPI', {
  getClips(skip: number, take: number, search: string): Promise<HTMLElement[]> {
    return ipcRenderer.invoke('clips:get', skip, take, search);
  }
});
