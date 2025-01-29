import { ClipboardDataPlugin } from "just-clipboard-manager-pdk";

export class TextPlugin extends ClipboardDataPlugin {
  override get id(): `${string}-${string}-${string}-${string}-${string}` {
    return 'd930d2cd-3fd9-4012-a363-120676e22afa';
  }

  override get formats(): readonly string[] {
    return ['CF_UNICODETEXT', 'CF_TEXT'];
  }

  override extractRepresentationData(data: Uint8Array): Uint8Array {
    throw new Error("Method not implemented.");
  }

  override getRepresentationDataElement(representationData: Uint8Array, document: Document): HTMLElement {
    const div = document.createElement('div');
    div.textContent = 'test clip';
    return div;
  }
}
