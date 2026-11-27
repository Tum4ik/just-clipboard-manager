using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using JustClipboardManager.Application.IPC.Common;
using JustClipboardManager.Application.IPC.Events;

namespace JustClipboardManager.Application.Services;

internal interface IEventsPublisher
{
  Task PublishAsync<TPayload>(Action<TPayload>? configurePayload = null) where TPayload : IMessage<TPayload>, new();
}


internal class EventsPublisher : IEventsPublisher
{
  private readonly IWindowingService _windowingService;

  public EventsPublisher(IWindowingService windowingService,
                         IIpcMessageTypesRegistry ipcMessageTypesRegistry)
  {
    _windowingService = windowingService;

    _eventJsonFormatter = new JsonFormatter(
      JsonFormatter.Settings.Default.WithTypeRegistry(ipcMessageTypesRegistry.EventPayloadsRegistry)
    );
  }


  private readonly JsonFormatter _eventJsonFormatter;


  public async Task PublishAsync<TPayload>(Action<TPayload>? configurePayload = null)
    where TPayload : IMessage<TPayload>, new()
  {
    var payload = new TPayload();
    if (configurePayload is not null)
    {
      configurePayload(payload);
    }
    var packedPayload = Google.Protobuf.WellKnownTypes.Any.Pack(payload);
    var appEvent = new AppEvent { Payload = packedPayload };
    var ipcMessage = new IpcMessage
    {
      Type = IpcMessage.Types.MessageType.Event,
      Body = Any.Pack(appEvent)
    };
    var ipcMessageJson = _eventJsonFormatter.Format(ipcMessage);

    await _windowingService.PublishEventForMainWindowAsync(ipcMessageJson);
    // todo: publish to other windows if needed
    // todo: maybe provide possibility to chose the window to publish to, or publish to all windows
  }
}
