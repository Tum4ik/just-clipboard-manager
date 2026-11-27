using System.Reflection;
using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JustClipboardManager.Application;

public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    var commandType = typeof(IAppCommand);
    var commandTypes = Assembly.GetExecutingAssembly()
      .GetTypes()
      .Where(
        t => commandType.IsAssignableFrom(t)
             && t.IsClass
             && !t.IsAbstract
      );
    foreach (var implType in commandTypes)
    {
      services.AddKeyedScoped(commandType, implType.Name, implType);
    }
    services.AddSingleton<ICommandsService, CommandsService>();

    services.AddSingleton<IEventsPublisher, EventsPublisher>();
    services.AddSingleton<IIpcMessageTypesRegistry, IpcMessageTypesRegistry>();
    services.AddSingleton<IClipsService, ClipsService>();

    return services;
  }
}
