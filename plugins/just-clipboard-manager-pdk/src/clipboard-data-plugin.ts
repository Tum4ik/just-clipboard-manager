export abstract class ClipboardDataPlugin {
  abstract get id(): `${string}-${string}-${string}-${string}-${string}`;
  abstract get formats(): readonly string[];
  abstract extractRepresentationData(data: Buffer): Buffer;
  abstract getRepresentationDataElement(data: Buffer): HTMLElement;
}
