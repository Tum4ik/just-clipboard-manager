import { inject, Service } from '@angular/core';
import { BackendProcessEventsService } from './backend-process-events-service';

@Service()
export class CommandsService {
  private readonly backendProcessEventsService = inject(BackendProcessEventsService);

  async getProductNameAsync(): Promise<string> {
    return await this.executeCommandAsync('GetProductNameCommand');
  }

  async getSelectedLanguageAsync(): Promise<string> {
    return await this.executeCommandAsync('GetSelectedLanguageCommand');
  }

  async setSelectedLanguageAsync(languageCode: string): Promise<void> {
    return await this.executeCommandAsync('SetSelectedLanguageCommand', languageCode);
  }


  private executeCommandAsync<T>(commandName: string, commandParameter?: any): Promise<T> {
    const eventId = crypto.randomUUID();
    const commandMessage: CommandMessage = {
      eventId: eventId,
      commandName: commandName,
      commandParameter: commandParameter,
    };
    return new Promise<T>(resolve => {
      const subscription = this.backendProcessEventsService.allEvents.subscribe(e => {
        if (e.id === eventId) {
          subscription.unsubscribe();
          resolve(e.payload);
        }
      });
      (window.chrome as any).webview.postMessage(commandMessage);
    });
  }
}


interface CommandMessage {
  eventId: string;
  commandName: string;
  commandParameter: any;
}
