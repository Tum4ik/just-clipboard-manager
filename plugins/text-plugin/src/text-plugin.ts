import { ClipboardDataPlugin, RepresentationData } from "just-clipboard-manager-pdk";

export class TextPlugin extends ClipboardDataPlugin {
  private readonly _representationFormats = ['CF_UNICODETEXT', 'CF_TEXT'];
  override get representationFormats(): readonly string[] {
    return this._representationFormats;
  }

  private readonly _formatsToSave = ['CF_UNICODETEXT', 'CF_TEXT'];
  override get formatsToSave(): readonly string[] {
    return this._formatsToSave;
  }

  private readonly encoder = new TextEncoder();
  private readonly utf8decoder = new TextDecoder();
  private readonly utf16decoder = new TextDecoder('utf-16');
  override extractRepresentationData(data: Uint8Array, format: string): { representationData: RepresentationData; searchLabel?: string; } {
    if (data.length <= 0) {
      return { representationData: { data } };
    }

    let text: string;
    if (format === 'CF_TEXT') {
      text = this.utf8decoder.decode(data);
    }
    else {
      text = this.utf16decoder.decode(data);
    }

    const firstLine = text.split(/\r?\n/).find(line => line.trim().length > 0) || '';
    let encoded = this.encoder.encode(firstLine);
    if (encoded[encoded.length - 1] === 0) {
      encoded = encoded.subarray(0, encoded.length - 1);
    }
    return { representationData: { data: encoded }, searchLabel: text };
  }

  override getRepresentationDataElement(representationData: RepresentationData, format: string, document: Document): HTMLElement {
    const div = document.createElement('div');
    div.textContent = this.utf8decoder.decode(representationData.data);
    div.style.textWrap = 'nowrap';
    div.style.overflow = 'hidden';
    div.style.textOverflow = 'ellipsis';
    return div;
  }

  override getFullDataPreviewElement(data: Uint8Array, format: string, document: Document): HTMLElement {
    let text: string;
    if (format === 'CF_TEXT') {
      text = this.utf8decoder.decode(data);
    }
    else {
      text = this.utf16decoder.decode(data);
    }

    const pre = document.createElement('pre');
    pre.textContent = text;
    pre.style.textWrap = 'nowrap';
    return pre;
  }
}
