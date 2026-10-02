import { Service } from '@angular/core';

@Service()
export class CommandsService {
  async getProductNameAsync(): Promise<string> {
    const commandMessage = { commandName: 'GetProductNameCommand' } as CommandMessage;
    return await this.executeCommandAsync(commandMessage);
  }


  private executeCommandAsync<T>(message: CommandMessage): Promise<T> {
    return new Promise<T>((resolve, reject) => {
      (window.chrome as any).webview.addEventListener('message', (e: MessageEvent) => {
        resolve(e.data);
      });
      (window.chrome as any).webview.postMessage(JSON.stringify(message));
    });
  }
}


interface CommandMessage {
  commandName: string;
  commandParameters: unknown;
}
