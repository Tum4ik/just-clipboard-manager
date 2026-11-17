import { Service } from '@angular/core';
import { fromBinary } from '@bufbuild/protobuf';
import { anyIs, anyUnpack } from '@bufbuild/protobuf/wkt';
import { AppEventSchema } from '@gen/events/app_event_pb';
import { LanguageChangedPayload, LanguageChangedPayloadSchema } from '@gen/events/payloads/language_changed_payload_pb';
import { Subject } from 'rxjs';

@Service()
export class BackendProcessEventsService {
  constructor() {
    (window.chrome as any).webview.addEventListener('sharedbufferreceived', (e: any) => {
      const buffer = e.getBuffer();
      const view = new Uint8Array(buffer);
      const appEvent = fromBinary(AppEventSchema, view);

      const packedPayload = appEvent.payload;
      if (packedPayload) {
        if (anyIs(packedPayload, LanguageChangedPayloadSchema)) {
          const payload = anyUnpack(packedPayload, LanguageChangedPayloadSchema);
          if (payload) {
            this.languageChangedSubject.next(payload);
          }
          else {
            // todo: report error - payload must be always present
          }
        }
      }
      else {
        // todo: report error - payload must be always present
      }
    });
  }

  private readonly allEventsSubject = new Subject<BackendProcessEvent>();
  readonly allEvents = this.allEventsSubject.asObservable();

  private readonly languageChangedSubject = new Subject<LanguageChangedPayload>();
  readonly languageChanged = this.languageChangedSubject.asObservable();
}


interface BackendProcessEvent {
  id?: string;
  name: string;
  payload: any;
}
