using Tum4ik.EventAggregator.Event;

namespace Tum4ik.EventAggregator;
internal class EventAggregator : IEventPublisher, IEventSubscriber
{
  private readonly SynchronizationContext? _synchronizationContext = SynchronizationContext.Current;
  private readonly Dictionary<Type, List<EventHandlerInfo>> _eventTypeToHandlerInfo = new();


  public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
  {
    foreach (var handler in PruneAndGetAliveHanlers(typeof(TEvent)))
    {
      switch (handler.ThreadOption)
      {
        case ThreadOption.PublisherThread:
          ExecuteHandler(handler, @event);
          break;
        case ThreadOption.MainThread:
          _synchronizationContext?.Post(d => ExecuteHandler(handler, @event), null);
          break;
        case ThreadOption.BackgroundThread:
          Task.Run(() => ExecuteHandler(handler, @event));
          break;
      }
    }
  }

  public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
  {
    foreach (var handler in PruneAndGetAliveHanlers(typeof(TEvent)))
    {
      switch (handler.ThreadOption)
      {
        case ThreadOption.PublisherThread:
          await ExecuteHandlerAsync(handler, @event);
          break;
        case ThreadOption.MainThread:
          var tcs = new TaskCompletionSource();
          _synchronizationContext?.Post(async d =>
          {
            await ExecuteHandlerAsync(handler, @event).ConfigureAwait(false);
            tcs.SetResult();
          }, null);
          await tcs.Task;
          break;
        case ThreadOption.BackgroundThread:
          await Task.Run(() => ExecuteHandlerAsync(handler, @event).ConfigureAwait(false));
          break;
      }
    }
  }


  public void Subscribe<TEvent>(Action<TEvent> handler,
                                ThreadOption threadOption = ThreadOption.PublisherThread,
                                bool keepSubscriberAlive = false)
    where TEvent : IEvent
  {
    SubscribeInternal(typeof(TEvent), handler, threadOption, keepSubscriberAlive);
  }

  public void Subscribe<TEvent>(Func<TEvent, Task> handler,
                                ThreadOption threadOption = ThreadOption.PublisherThread,
                                bool keepSubscriberAlive = false)
    where TEvent : IEvent
  {
    SubscribeInternal(typeof(TEvent), handler, threadOption, keepSubscriberAlive);
  }


  public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
  {
    UnsubscribeInternal(typeof(TEvent), handler);
  }

  public void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
  {
    UnsubscribeInternal(typeof(TEvent), handler);
  }


  private static void ExecuteHandler<TEvent>(EventHandlerInfo handler, TEvent @event)
  {
    switch (handler.HandlerAction)
    {
      case Action<TEvent> action:
        action(@event);
        break;
      case Func<TEvent, Task> func:
        func(@event);
        break;
    }
  }

  private static async Task ExecuteHandlerAsync<TEvent>(EventHandlerInfo handler, TEvent @event)
  {
    switch (handler.HandlerAction)
    {
      case Action<TEvent> action:
        action(@event);
        break;
      case Func<TEvent, Task> func:
        await func(@event).ConfigureAwait(false);
        break;
    }
  }


  private List<EventHandlerInfo> PruneAndGetAliveHanlers(Type eventType)
  {
    var aliveHandlers = new List<EventHandlerInfo>();
    lock (_eventTypeToHandlerInfo)
    {
      if (_eventTypeToHandlerInfo.TryGetValue(eventType, out var handlerInfos))
      {
        for (var i = handlerInfos.Count - 1; i >= 0; i--)
        {
          if (handlerInfos[i].IsAlive)
          {
            aliveHandlers.Add(handlerInfos[i]);
          }
          else
          {
            handlerInfos.RemoveAt(i);
          }
        }
      }
    }

    return aliveHandlers;
  }


  private void SubscribeInternal(Type eventType, Delegate handler, ThreadOption threadOption, bool keepSubscriberAlive)
  {
    var eventHandlerInfo = new EventHandlerInfo(eventType, handler, threadOption, keepSubscriberAlive);
    lock (_eventTypeToHandlerInfo)
    {
      if (!_eventTypeToHandlerInfo.TryGetValue(eventType, out var handlerInfos))
      {
        handlerInfos = new();
        _eventTypeToHandlerInfo[eventType] = handlerInfos;
      }

      handlerInfos.Add(eventHandlerInfo);
    }
  }


  private void UnsubscribeInternal(Type eventType, Delegate handler)
  {
    lock (_eventTypeToHandlerInfo)
    {
      if (_eventTypeToHandlerInfo.TryGetValue(eventType, out var handlerInfos))
      {
        var handlerToRemove = handlerInfos.FirstOrDefault(hi => hi.HandlerAction == handler);
        if (handlerToRemove is not null)
        {
          handlerInfos.Remove(handlerToRemove);
        }
      }
    }
  }
}
