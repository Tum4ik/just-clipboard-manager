using System;
using System.Threading.Tasks;
using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.Services;

namespace JustClipboardManager.Services;

internal class EventsPublisher : IEventsPublisher
{
  private readonly IWindowingService _windowingService;

  public EventsPublisher(IWindowingService windowingService)
  {
    _windowingService = windowingService;
  }

  public async Task PublishAsync<TEvent>() where TEvent : IAppEvent, new()
  {
    var @event = new TEvent();
    await _windowingService.PublishEventForMainWindowAsync(@event);
    // todo: publish to other windows if needed
    // todo: maybe provide possibility to chosse the window to publish to, or publish to all windows
  }


  public async Task PublishAsync<TEvent, TPayload>(Func<TPayload> payloadFactory) where TEvent : IAppEvent<TPayload>, new()
  {
    var payload = payloadFactory();
    var @event = new TEvent()
    {
      Payload = payload
    };
    await _windowingService.PublishEventForMainWindowAsync(@event);
    // todo: publish to other windows if needed
    // todo: maybe provide possibility to chosse the window to publish to, or publish to all windows
  }
}
