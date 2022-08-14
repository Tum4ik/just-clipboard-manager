namespace Tum4ik.EventAggregator.Event;
public abstract class EventBase
{
  internal SynchronizationContext? SynchronizationContext { get; init; }
  private readonly List<EventHandlerInfo> _handlers = new();


  protected void PublishInternal(object? payload = null)
  {
    foreach (var handler in PruneAndGetAliveHanlers())
    {
      var handlerAction = handler.HandlerAction;
      if (handlerAction is null)
      {
        continue;
      }

      switch (handler.ThreadOption)
      {
        case ThreadOption.PublisherThread:
          ExecuteHandler(handlerAction, payload);
          break;
        case ThreadOption.MainThread:
          if (SynchronizationContext is not null)
          {
            SynchronizationContext.Post(d => ExecuteHandler(handlerAction, payload), null);
          }
          else
          {
            ExecuteHandler(handlerAction, payload);
          }
          break;
        case ThreadOption.BackgroundThread:
          Task.Run(() => ExecuteHandler(handlerAction, payload));
          break;
      }
    }
  }


  protected abstract void ExecuteHandler(Delegate handler, object? payload);


  protected void SubscribeInternal(Delegate handler,
                                            ThreadOption threadOption = ThreadOption.PublisherThread,
                                            bool keepSubscriberAlive = false)
  {
    var eventHandlerInfo = new EventHandlerInfo(handler, threadOption, keepSubscriberAlive);
    lock (_handlers)
    {
      _handlers.Add(eventHandlerInfo);
    }
  }


  protected void UnsubscribeInternal(Delegate handler)
  {
    lock (_handlers)
    {
      var handlerToRemove = _handlers.FirstOrDefault(hi => hi.HandlerAction == handler);
      if (handlerToRemove is not null)
      {
        _handlers.Remove(handlerToRemove);
      }
    }
  }


  private List<EventHandlerInfo> PruneAndGetAliveHanlers()
  {
    var aliveHandlers = new List<EventHandlerInfo>();
    lock (_handlers)
    {
      for (var i = _handlers.Count - 1; i >= 0; i--)
      {
        if (_handlers[i].IsAlive)
        {
          aliveHandlers.Add(_handlers[i]);
        }
        else
        {
          _handlers.RemoveAt(i);
        }
      }
    }

    return aliveHandlers;
  }
}
