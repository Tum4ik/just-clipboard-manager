using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.Common.IpcPayloads.Events;

namespace JustClipboardManager.Application.Events;

internal class LanguageChangedEvent : IAppEvent<LanguageChangedPayload>
{
  public string Name { get; } = nameof(LanguageChangedEvent);
  public LanguageChangedPayload? Payload { get; init; }
}
