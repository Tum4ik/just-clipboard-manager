using JustClipboardManager.Application.Common.Abstractions;

namespace JustClipboardManager.Application.Events;

internal class LanguageChangedEvent : IAppEvent<LanguageChangedPayload>
{
  public string Name { get; } = nameof(LanguageChangedEvent);
  public LanguageChangedPayload? Payload { get; init; }
}


internal class LanguageChangedPayload
{
  public required string LanguageCode { get; init; }
}
