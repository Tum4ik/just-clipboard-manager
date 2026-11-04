using JustClipboardManager.Application.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace JustClipboardManager.Application.Services;

public interface ICommandsService
{
  Task<string?> ExecuteCommandAsync(string commandName,
                                    string? commandParameter,
                                    CancellationToken cancellationToken = default);
}


internal class CommandsService : ICommandsService
{
  private readonly IServiceProvider _serviceProvider;

  public CommandsService(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }


  public async Task<string?> ExecuteCommandAsync(string commandName,
                                                 string? commandParameter,
                                                 CancellationToken cancellationToken = default)
  {
    var command = _serviceProvider.GetRequiredKeyedService<IAppCommand>(commandName);
    return await command.ExecuteAsync(commandParameter, cancellationToken);
  }
}
