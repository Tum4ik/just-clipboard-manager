import { ClipboardDataPlugin } from "just-clipboard-manager-pdk";

export class TextPlugin extends ClipboardDataPlugin {

  private readonly _id = 'd930d2cd-3fd9-4012-a363-120676e22afa';
  override get id(): `${string}-${string}-${string}-${string}-${string}` {
    return this._id;
  }

  private readonly _formats = ['CF_UNICODETEXT', 'CF_TEXT'];
  override get formats(): readonly string[] {
    // the statement should be verified when Linux is supported
    // UNICODETEXT typically is UTF-16 with CRLF on Windows and UTF-8 with LF on Linux
    return this._formats;
  }

  private readonly encoder = new TextEncoder();
  private readonly utf8decoder = new TextDecoder();
  private readonly utf16decoder = new TextDecoder('utf-16');
  override extractRepresentationData(data: Uint8Array, format: string): Uint8Array {
    if (data.length <= 0) {
      return data;
    }

    let text: string;
    if (format === 'CF_TEXT') {
      text = this.utf8decoder.decode(data);
    }
    else {
      text = this.utf16decoder.decode(data);
    }

    const firstLine = text.split(/\r?\n/)[0];
    let encoded = this.encoder.encode(firstLine);
    if (encoded[encoded.length - 1] === 0) {
      encoded = encoded.subarray(0, encoded.length - 1);
    }
    return encoded;
  }

  override getRepresentationDataElement(representationData: Uint8Array, format: string, document: Document): HTMLElement {
    const div = document.createElement('div');
    div.textContent = this.utf8decoder.decode(representationData);
    div.style.overflow = 'hidden';
    div.style.textOverflow = 'ellipsis';
    return div;
  }

  override getSearchLabel(data: Uint8Array, format: string): string {
    if (format === 'CF_TEXT') {
      return this.utf8decoder.decode(data);
    }
    return this.utf16decoder.decode(data);
  }
}
