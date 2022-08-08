using Tum4ik.EventAggregator.Event;

namespace Tum4ik.EventAggregator;
internal class EventPublisher : IEventPublisher
{
  private readonly EventAggregator _eventAggregator;

  public EventPublisher(EventAggregator eventAggregator)
  {
    _eventAggregator = eventAggregator;
  }


  public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
  {
    _eventAggregator.Publish(@event);
  }

  public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
  {
    await _eventAggregator.PublishAsync(@event).ConfigureAwait(false);
  }
}
