namespace Tum4ik.EventAggregator.Event;
public abstract class PubSubEvent : EventBase
{
  public void Publish()
  {
    PublishInternal();
  }


  protected override async void ExecuteHandler(Delegate handler, object? payload)
  {
    switch (handler)
    {
      case Action action:
        action();
        break;
      case Func<Task> func:
        try
        {
          await func().ConfigureAwait(false);
        }
        catch (Exception e)
        {
          SynchronizationContext?.Post(_ => throw e, null);
        }
        break;
    }
  }


  public void Subscribe(Action handler,
                        ThreadOption threadOption = ThreadOption.PublisherThread,
                        bool keepSubscriberAlive = false)
  {
    SubscribeInternal(handler, threadOption, keepSubscriberAlive);
  }


  public void Subscribe(Func<Task> handler,
                        ThreadOption threadOption = ThreadOption.PublisherThread,
                        bool keepSubscriberAlive = false)
  {
    SubscribeInternal(handler, threadOption, keepSubscriberAlive);
  }


  public void Unsubscribe(Action handler)
  {
    UnsubscribeInternal(handler);
  }


  public void Unsubscribe(Func<Task> handler)
  {
    UnsubscribeInternal(handler);
  }
}


public abstract class PubSubEvent<TPayload> : EventBase
{
  public void Publish(TPayload payload)
  {
    PublishInternal(payload);
  }


  protected override async void ExecuteHandler(Delegate handler, object? payload)
  {
    if (payload is null)
    {
      return;
    }

    switch (handler)
    {
      case Action<TPayload> action:
        action((TPayload)payload);
        break;
      case Func<TPayload, Task> func:
        try
        {
          await func((TPayload) payload).ConfigureAwait(false);
        }
        catch (Exception e)
        {
          SynchronizationContext?.Post(_ => throw e, null);
        }
        break;
    }
  }


  public void Subscribe(Action<TPayload> handler,
                        ThreadOption threadOption = ThreadOption.PublisherThread,
                        bool keepSubscriberAlive = false)
  {
    SubscribeInternal(handler, threadOption, keepSubscriberAlive);
  }


  public void Subscribe(Func<TPayload, Task> handler,
                        ThreadOption threadOption = ThreadOption.PublisherThread,
                        bool keepSubscriberAlive = false)
  {
    SubscribeInternal(handler, threadOption, keepSubscriberAlive);
  }


  public void Unsubscribe(Action<TPayload> handler)
  {
    UnsubscribeInternal(handler);
  }


  public void Unsubscribe(Func<TPayload, Task> handler)
  {
    UnsubscribeInternal(handler);
  }
}
