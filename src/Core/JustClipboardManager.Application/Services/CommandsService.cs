using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.IPC.Commands;
using JustClipboardManager.Application.IPC.Common;
using Microsoft.Extensions.DependencyInjection;

namespace JustClipboardManager.Application.Services;

public interface ICommandsService
{
  Task<string> ExecuteCommandAsync(string webMessageJson,
                                   IServiceScope serviceScope,
                                   CancellationToken cancellationToken = default);
}


internal class CommandsService : ICommandsService
{
  private readonly IServiceProvider _serviceProvider;

  public CommandsService(IServiceProvider serviceProvider,
                         IIpcMessageTypesRegistry ipcMessageTypesRegistry)
  {
    _serviceProvider = serviceProvider;

    _commandJsonParser = new JsonParser(
      JsonParser.Settings.Default.WithTypeRegistry(ipcMessageTypesRegistry.CommandParametersRegistry)
    );
    _commandJsonFormatter = new JsonFormatter(
      JsonFormatter.Settings.Default.WithTypeRegistry(ipcMessageTypesRegistry.CommandResultsRegistry)
    );
  }


  private readonly JsonParser _commandJsonParser;
  private readonly JsonFormatter _commandJsonFormatter;


  public async Task<string> ExecuteCommandAsync(string webMessageJson,
                                                IServiceScope serviceScope,
                                                CancellationToken cancellationToken = default)
  {
    var incomingIpcMessage = _commandJsonParser.Parse<IpcMessage>(webMessageJson);
    var commandRequest = incomingIpcMessage.Body.Unpack<AppCommandRequest>();

    var command = serviceScope.ServiceProvider.GetRequiredKeyedService<IAppCommand>(commandRequest.Name);
    var result = await command.ExecuteAsync(commandRequest.Parameter, cancellationToken);

    var commandResult = new AppCommandResult
    {
      RequestId = commandRequest.RequestId,
      Result = result
    };
    var ipcMessage = new IpcMessage
    {
      Type = IpcMessage.Types.MessageType.CommandResult,
      Body = Any.Pack(commandResult)
    };

    var commandResultJson = _commandJsonFormatter.Format(ipcMessage);
    return commandResultJson;
  }
}
