namespace Tum4ik.EventAggregator.Event;

public interface IEvent
{
}

public interface IEvent<TPayload> : IEvent
{
  TPayload Payload { get; init; }
}
