import { emit, EventCallback, EventName, listen, UnlistenFn } from '@tauri-apps/api/event';

export abstract class GlobalStateService {
  protected registerGlobalObservable<T>(event: EventName, handler: EventCallback<T>): GlobalStateSetter<T> {
    return new GlobalStateSetter<T>(event, listen<T>(event, handler));
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
