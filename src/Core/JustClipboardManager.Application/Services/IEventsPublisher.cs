using Google.Protobuf;

namespace JustClipboardManager.Application.Services;

public interface IEventsPublisher
{
  Task PublishAsync<TPayload>(Action<TPayload>? configurePayload = null) where TPayload : IMessage<TPayload>, new();
}
