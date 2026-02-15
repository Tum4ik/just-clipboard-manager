import { emit, EventCallback, EventName, listen, UnlistenFn } from '@tauri-apps/api/event';

export abstract class GlobalStateService {
  private static registeredEvents = new Set<EventName>();

  protected registerGlobalObservable<T>(event: EventName, onGloballyChangedCallback: EventCallback<T>): GlobalStateSetter<T> {
    if (GlobalStateService.registeredEvents.has(event)) {
      throw new Error('The global event is already registered.');
    }
    GlobalStateService.registeredEvents.add(event);
    return new GlobalStateSetter<T>(event, listen<T>(event, onGloballyChangedCallback));
  }
}


export class GlobalStateSetter<T> {
  constructor(
    private readonly event: EventName,
    private readonly unlistenPromise: Promise<UnlistenFn>,
  ) { }

  private unlisten?: UnlistenFn;

  async setAsync(value: T): Promise<void> {
    if (!this.unlisten) {
      this.unlisten = await this.unlistenPromise;
    }
    await emit(this.event, value);
  }

  dispose(): void {
    this.unlisten?.();
  }
}
