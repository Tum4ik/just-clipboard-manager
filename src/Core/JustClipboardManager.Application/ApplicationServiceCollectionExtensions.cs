using System.Reflection;
using JustClipboardManager.Application.Common.Abstractions;
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
      // todo: need a possibility to specify lifetime for each command
      services.AddKeyedTransient(commandType, implType.Name, implType);
    }

    return services;
  }
}
