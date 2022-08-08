using Tum4ik.EventAggregator.Event;

namespace Tum4ik.EventAggregator;
public interface IEventSubscriber
{
  void Subscribe<TEvent>(Action<TEvent> handler, 
                         ThreadOption threadOption = ThreadOption.PublisherThread, 
                         bool keepSubscriberAlive = false) 
    where TEvent : IEvent;

  void Subscribe<TEvent>(Func<TEvent, Task> handler, 
                         ThreadOption threadOption = ThreadOption.PublisherThread, 
                         bool keepSubscriberAlive = false) 
    where TEvent : IEvent;


  void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
  void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;
}
