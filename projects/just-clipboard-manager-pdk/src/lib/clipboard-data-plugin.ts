import { ComponentRef } from "@angular/core";

export abstract class ClipboardDataPlugin<TComponent> {
  abstract get pluginId(): `${string}-${string}-${string}-${string}-${string}`;
  abstract get formats(): readonly string[];
  abstract get representationDataComponent(): ComponentRef<TComponent>;
  abstract processData(): Buffer;
}
