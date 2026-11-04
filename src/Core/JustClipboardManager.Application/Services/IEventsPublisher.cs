using JustClipboardManager.Application.Common.Abstractions;

namespace JustClipboardManager.Application.Services;

public interface IEventsPublisher
{
  Task PublishAsync<TEvent>() where TEvent : IAppEvent, new();
  Task PublishAsync<TEvent, TPayload>(Func<TPayload> payloadFactory) where TEvent : IAppEvent<TPayload>, new();
}
