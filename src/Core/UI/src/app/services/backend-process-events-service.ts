import { Service } from '@angular/core';
import { LanguageChangedPayload } from '@gen/events/language_changed_payload_pb';
import { Subject } from 'rxjs';

@Service()
export class BackendProcessEventsService {
  constructor() {
    (window.chrome as any).webview.addEventListener('message', (e: MessageEvent<BackendProcessEvent>) => {
      this.allEventsSubject.next(e.data);
      if (e.data.name === 'LanguageChangedEvent') {
        this.languageChangedSubject.next(e.data.payload);
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
