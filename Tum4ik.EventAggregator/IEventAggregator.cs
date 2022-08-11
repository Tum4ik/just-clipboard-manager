using Tum4ik.EventAggregator.Event;

namespace Tum4ik.EventAggregator;
public interface IEventAggregator
{
  TEvent GetEvent<TEvent>() where TEvent : EventBase, new();
}
