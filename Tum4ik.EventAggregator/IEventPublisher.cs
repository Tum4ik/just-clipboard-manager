using Tum4ik.EventAggregator.Event;

namespace Tum4ik.EventAggregator;

public interface IEventPublisher
{
  void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
  Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
}
