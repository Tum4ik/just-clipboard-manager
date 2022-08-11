using Tum4ik.EventAggregator.Event;

namespace Tum4ik.EventAggregator;
internal class EventAggregator : IEventAggregator
{
  private readonly SynchronizationContext? _synchronizationContext = SynchronizationContext.Current;
  private readonly Dictionary<Type, EventBase> _events = new();


  public TEvent GetEvent<TEvent>() where TEvent : EventBase, new()
  {
    lock (_events)
    {
      if (_events.TryGetValue(typeof(TEvent), out var @event))
      {
        return (TEvent) @event;
      }

      @event = new TEvent()
      {
        SynchronizationContext = _synchronizationContext
      };
      _events[typeof(TEvent)] = @event;

      return (TEvent) @event;
    }
  }
}
