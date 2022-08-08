using Microsoft.Extensions.DependencyInjection;

namespace Tum4ik.EventAggregator.Extensions;
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddEventAggregator(this IServiceCollection services)
  {
    return services
      .AddSingleton<EventAggregator>()
      .AddSingleton<IEventPublisher, EventPublisher>()
      .AddSingleton<IEventSubscriber, EventSubscriber>();
  }
}
