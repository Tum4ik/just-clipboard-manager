using Tum4ik.EventAggregator.Event;

namespace Tum4ik.EventAggregator;
internal class EventSubscriber : IEventSubscriber
{
  private readonly EventAggregator _eventAggregator;

  public EventSubscriber(EventAggregator eventAggregator)
  {
    _eventAggregator = eventAggregator;
  }


  public void Subscribe<TEvent>(Action<TEvent> handler, 
                                ThreadOption threadOption = ThreadOption.PublisherThread, 
                                bool keepSubscriberAlive = false) 
    where TEvent : IEvent
  {
    _eventAggregator.Subscribe(handler, threadOption, keepSubscriberAlive);
  }

  public void Subscribe<TEvent>(Func<TEvent, Task> handler, 
                                ThreadOption threadOption = ThreadOption.PublisherThread, 
                                bool keepSubscriberAlive = false)
    where TEvent : IEvent
  {
    _eventAggregator.Subscribe(handler, threadOption, keepSubscriberAlive);
  }


  public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
  {
    _eventAggregator.Unsubscribe(handler);
  }

  public void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
  {
    _eventAggregator.Unsubscribe(handler);
  }
}
