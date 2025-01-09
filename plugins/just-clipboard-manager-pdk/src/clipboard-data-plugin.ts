import { Clipboard } from 'electron';

export abstract class ClipboardDataPlugin {
  abstract get id(): `${string}-${string}-${string}-${string}-${string}`;
  abstract get formats(): readonly string[];
  abstract extractData(clipboard: Clipboard): Buffer;
  abstract insertData(clipboard: Clipboard, data: Buffer): void;
  abstract extractRepresentationData(clipboard: Clipboard): Buffer;
  abstract getRepresentationDataElement(representationData: Buffer, document: Document): HTMLElement;
}
