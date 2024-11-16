import { ComponentRef } from '@angular/core';
import { Buffer } from 'buffer';
import { ClipboardDataPlugin } from 'just-clipboard-manager-pdk';
import { TextClipComponent } from './components/text-clip/text-clip.component';

export class TextPlugin extends ClipboardDataPlugin<TextClipComponent> {
  override get pluginId(): `${string}-${string}-${string}-${string}-${string}` {
    return 'd930d2cd-3fd9-4012-a363-120676e22afa';
  }

  override get formats(): readonly string[] {
    return ['text/plain'];
  }

  override get representationDataComponent(): ComponentRef<TextClipComponent> {
    throw new Error('Method not implemented.');
  }

  override processData(): Buffer {
    return Buffer.from('test');
  }
}
