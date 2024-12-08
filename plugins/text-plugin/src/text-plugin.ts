import { ClipboardDataPlugin } from "just-clipboard-manager-pdk";
import os from 'os';

export class TextPlugin extends ClipboardDataPlugin {
  override get id(): `${string}-${string}-${string}-${string}-${string}` {
    return 'd930d2cd-3fd9-4012-a363-120676e22afa';
  }

  override get formats(): readonly string[] {
    return ['text/plain'];
  }

  override extractData(clipboard: Electron.CrossProcessExports.Clipboard): Buffer {
    const text = clipboard.readText();
    return Buffer.from(text, 'utf-8');
  }

  override insertData(clipboard: Electron.CrossProcessExports.Clipboard, data: Buffer): void {
    const text = data.toString('utf-8');
    clipboard.writeText(text);
  }

  override extractRepresentationData(clipboard: Electron.CrossProcessExports.Clipboard): Buffer {
    const text = clipboard.readText();
    const firstLine = text.split(os.EOL)[0];
    return Buffer.from(firstLine, 'utf-8');
  }

  override getRepresentationDataElement(representationData: Buffer): HTMLElement {
    const div = document.createElement('div');
    div.textContent = representationData.toString('utf-8');
    return div;
  }
}
