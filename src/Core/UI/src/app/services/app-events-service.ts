import { Service } from '@angular/core';
import { createRegistry, enumFromJson, fromJson, Message } from '@bufbuild/protobuf';
import { GenMessage } from '@bufbuild/protobuf/codegenv2';
import { anyIs, anyUnpack } from '@bufbuild/protobuf/wkt';
import { IpcMessage_MessageType, IpcMessage_MessageTypeSchema, IpcMessageSchema } from '@gen/common/ipc_message_pb';
import { AppEventSchema } from '@gen/events/app_event_pb';
import { LanguageChangedPayloadSchema } from '@gen/events/payloads/language_changed_payload_pb';
import { Observable, Subject } from 'rxjs';


const EventsMap = {
  languageChanged: { schema: LanguageChangedPayloadSchema },
} satisfies Record<string, AppEvent<any>>;


export interface AppEventsService extends Readonly<{
  [K in keyof EventPayloads]: Observable<EventPayloads[K]>;
}> { }


@Service()
export class AppEventsService {
  private readonly subjects = new Map<keyof EventPayloads, Subject<any>>();

  constructor() {
    const schemas: GenMessage<Message>[] = [];
    for (const key of Object.keys(EventsMap) as (keyof EventPayloads)[]) {
      schemas.push(EventsMap[key].schema);

      const subject = new Subject<any>();
      this.subjects.set(key, subject);

      Object.defineProperty(this, key, {
        value: subject.asObservable(),
        writable: false,
        enumerable: true,
      });
    }

    (window.chrome as any).webview.addEventListener('message', (e: MessageEvent<any>) => {
      const ipcMessageType = enumFromJson(IpcMessage_MessageTypeSchema, e.data.type);
      if (ipcMessageType !== IpcMessage_MessageType.EVENT) {
        return;
      }

      const ipcMessage = fromJson(IpcMessageSchema, e.data, {
        registry: createRegistry(AppEventSchema, ...schemas)
      });
      const appEvent = anyUnpack(ipcMessage.body!, AppEventSchema)!;
      const payload = appEvent.payload;
      if (!payload) {
        return;
      }

      if (anyIs(payload, LanguageChangedPayloadSchema)) {
        const languageChangedPayload = anyUnpack(payload, LanguageChangedPayloadSchema)!;
        this.emit('languageChanged', languageChangedPayload);
      }
      // else if
    });
  }

  private emit<K extends keyof EventPayloads>(event: K, payload: EventPayloads[K]): void {
    this.subjects.get(event)?.next(payload);
  }
}


interface AppEvent<T extends Message> {
  schema: GenMessage<T>;
}

type EventPayloads = {
  [K in keyof typeof EventsMap]: (typeof EventsMap)[K] extends AppEvent<infer T> ? T : never;
};
