import { Service } from '@angular/core';
import { Subject } from 'rxjs';

@Service()
export class BackendProcessEventsService {
  constructor() {
    (window.chrome as any).webview.addEventListener('message', (e: MessageEvent) => {
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


// todo: generate via gRPC
interface LanguageChangedPayload {
  languageCode: string;
}
