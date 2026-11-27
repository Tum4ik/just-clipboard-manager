import { Service } from '@angular/core';
import { create, createRegistry, DescMessage, enumFromJson, fromJson, Message, toJson } from '@bufbuild/protobuf';
import { GenMessage } from '@bufbuild/protobuf/codegenv2';
import { Any, anyPack, anyUnpack } from '@bufbuild/protobuf/wkt';
import { AppCommandRequestSchema } from '@gen/commands/app_command_request_pb';
import { AppCommandResultSchema } from '@gen/commands/app_command_result_pb';
import { GetProductNameResult, GetProductNameResultSchema } from '@gen/commands/get_product_name/get_product_name_result_pb';
import { GetSelectedLanguageResult, GetSelectedLanguageResultSchema } from '@gen/commands/get_selected_language/get_selected_language_result_pb';
import { SetSelectedLanguageParameterSchema } from '@gen/commands/set_selected_language/set_selected_language_parameter_pb';
import { Empty, EmptySchema } from '@gen/common/empty_pb';
import { IpcMessage_MessageType, IpcMessage_MessageTypeSchema, IpcMessageSchema } from '@gen/common/ipc_message_pb';

@Service()
export class AppCommandsService {
  async getProductNameAsync(): Promise<string> {
    const result = await this.executeCommandAsync<Empty, GetProductNameResult>(
      'GetProductNameCommand',
      EmptySchema, create(EmptySchema),
      GetProductNameResultSchema
    );
    return result.productName;
  }

  async getSelectedLanguageAsync(): Promise<string> {
    const result = await this.executeCommandAsync<Empty, GetSelectedLanguageResult>(
      'GetSelectedLanguageCommand',
      EmptySchema, create(EmptySchema),
      GetSelectedLanguageResultSchema
    );
    return result.languageCode;
  }

  async setSelectedLanguageAsync(languageCode: string): Promise<void> {
    await this.executeCommandAsync(
      'SetSelectedLanguageCommand',
      SetSelectedLanguageParameterSchema, create(SetSelectedLanguageParameterSchema, { languageCode: languageCode }),
      EmptySchema
    );
  }


  private readonly webview = (window.chrome as any).webview;

  private async executeCommandAsync<TParameter extends Message, TResult extends Message>(
    commandName: string,
    commandParameterSchema: DescMessage,
    commandParameter: TParameter,
    resultSchema: GenMessage<TResult>
  ): Promise<TResult> {
    const requestId = crypto.randomUUID();

    const appCommandRequest = create(AppCommandRequestSchema, {
      name: commandName,
      requestId: requestId,
      parameter: anyPack(commandParameterSchema, commandParameter)
    });
    const outgoingIpcMessage = create(IpcMessageSchema, {
      type: IpcMessage_MessageType.COMMAND_REQUEST,
      body: anyPack(AppCommandRequestSchema, appCommandRequest)
    });
    const outgoingIpcMessageJson = toJson(IpcMessageSchema, outgoingIpcMessage, {
      registry: createRegistry(AppCommandRequestSchema, commandParameterSchema)
    });

    const packedCommandResult = await new Promise<Any>(resolve => {
      const handleMessage = (e: MessageEvent<any>) => {
        const ipcMessageType = enumFromJson(IpcMessage_MessageTypeSchema, e.data.type);
        if (ipcMessageType !== IpcMessage_MessageType.COMMAND_RESULT) {
          return;
        }

        const ipcMessage = fromJson(IpcMessageSchema, e.data, {
          registry: createRegistry(AppCommandResultSchema, resultSchema)
        });
        const commandResult = anyUnpack(ipcMessage.body!, AppCommandResultSchema)!;
        if (commandResult.requestId !== requestId) {
          return;
        }

        this.webview.removeEventListener('message', handleMessage);
        resolve(commandResult.result!);
      };
      this.webview.addEventListener('message', handleMessage);

      this.webview.postMessage(outgoingIpcMessageJson);
    });

    const commandResult = anyUnpack(packedCommandResult, resultSchema)!;
    return commandResult;
  }
}
