namespace JustClipboardManager.Application.Common.Abstractions;

public interface IAppEvent
{
  string Name { get; }
}


public interface IAppEvent<T> : IAppEvent
{
  T? Payload { get; init; }
}
