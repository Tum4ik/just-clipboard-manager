export abstract class ClipboardDataPlugin {
  abstract get id(): `${string}-${string}-${string}-${string}-${string}`;
  abstract get formats(): readonly string[];
  abstract extractRepresentationData(data: Uint8Array, format: string): Uint8Array;
  abstract getRepresentationDataElement(representationData: Uint8Array, format: string, document: Document): HTMLElement;
}
