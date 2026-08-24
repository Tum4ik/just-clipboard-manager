import { Service } from '@angular/core';
import { emit, EventCallback, EventName, listen, UnlistenFn } from '@tauri-apps/api/event';

/**
 * Service designed to synchronize state changes across multiple windows within a Tauri-based application.
 */
@Service()
export class GlobalStateService {
  private static registeredEvents = new Set<EventName>();

  /**
   * Registers a global observable event and starts listening to it.
   * Ensures that a global state event is registered only once to prevent duplicate listeners.
   *
   * @template T The type of the state value.
   * @param event The unique event name/identifier.
   * @param onGloballyChangedCallback Callback function invoked when the event is triggered.
   * @returns A `GlobalStateSetter` instance to broadcast updates or clean up the listener.
   * @throws Error if the event is already registered.
   */
  registerGlobalObservable<T>(event: EventName, onGloballyChangedCallback: EventCallback<T>): GlobalStateSetter<T> {
    if (GlobalStateService.registeredEvents.has(event)) {
      throw new Error('The global event is already registered.');
    }
    GlobalStateService.registeredEvents.add(event);
    return new GlobalStateSetter<T>(event, listen<T>(event, onGloballyChangedCallback));
  }
}


/**
 * Helper class that manages the emission of global state changes and cleanup of the underlying Tauri event listener.
 *
 * @template T The type of the state value.
 */
export class GlobalStateSetter<T> {
  constructor(
    private readonly event: EventName,
    private readonly unlistenPromise: Promise<UnlistenFn>,
  ) { }

  private unlisten?: UnlistenFn;

  /**
   * Asynchronously broadcasts the new state value to all listening windows and components.
   *
   * @param value The new state value to emit.
   */
  async invokeAsync(value: T): Promise<void> {
    if (!this.unlisten) {
      this.unlisten = await this.unlistenPromise;
    }
    await emit(this.event, value);
  }

  /**
   * Cleans up the event subscription by calling the Tauri unlisten function.
   */
  dispose(): void {
    this.unlisten?.();
  }
}
