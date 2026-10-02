using System.Text.Json;
using JustClipboardManager.Application.Common;
using JustClipboardManager.Application.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace JustClipboardManager.Application.Services;

public interface ICommandsService
{
  Task<string?> ExecuteCommandAsync(string message, CancellationToken cancellationToken = default);
}


internal class CommandsService : ICommandsService
{
  private readonly IServiceProvider _serviceProvider;

  public CommandsService(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  private static readonly JsonSerializerOptions s_deserializationOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public async Task<string?> ExecuteCommandAsync(string message, CancellationToken cancellationToken = default)
  {
    var commandMessage = JsonSerializer.Deserialize<CommandMessage>(message, s_deserializationOptions);
    ArgumentNullException.ThrowIfNull(commandMessage);
    var command = _serviceProvider.GetRequiredKeyedService<IAppCommand>(commandMessage.CommandName);
    var commandResult = await command.ExecuteAsync(commandMessage.CommandParameter, cancellationToken);
    return commandResult;
  }
}
