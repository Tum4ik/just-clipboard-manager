using System;
using System.Threading.Tasks;
using Google.Protobuf;
using JustClipboardManager.Application.IPC.Events;
using JustClipboardManager.Application.Services;

namespace JustClipboardManager.Services;

internal class EventsPublisher : IEventsPublisher
{
  private readonly IWindowingService _windowingService;

  public EventsPublisher(IWindowingService windowingService)
  {
    _windowingService = windowingService;
  }

  public async Task PublishAsync<TPayload>(Action<TPayload>? configurePayload = null) where TPayload : IMessage<TPayload>, new()
  {
    var payload = new TPayload();
    if (configurePayload is not null)
    {
      configurePayload(payload);
    }
    var packedPayload = Google.Protobuf.WellKnownTypes.Any.Pack(payload, "jcm");
    var appEvent = new AppEvent { Payload = packedPayload };
    
    await _windowingService.PublishEventForMainWindowAsync(appEvent);
    // todo: publish to other windows if needed
    // todo: maybe provide possibility to chose the window to publish to, or publish to all windows
  }
}
