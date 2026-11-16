using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.Common.IpcPayloads.Events;
using JustClipboardManager.Application.Events;
using JustClipboardManager.Application.Services;
using JustClipboardManager.Domain.Common;

namespace JustClipboardManager.Application.Commands.Settings.Interface;

internal class SetSelectedLanguageCommand : AppCommand<string, None>
{
  private readonly ISettingsService _settingsService;
  private readonly IEventsPublisher _eventsPublisher;

  public SetSelectedLanguageCommand(ISettingsService settingsService,
                                    IEventsPublisher eventsPublisher)
  {
    _settingsService = settingsService;
    _eventsPublisher = eventsPublisher;
  }

  protected override async Task<None> ExecuteInternalAsync(string parameter, CancellationToken cancellationToken = default)
  {
    _settingsService.UserSettings.SelectedLanguageCode = parameter;
    await _settingsService.SaveChangesAsync();
    await _eventsPublisher.PublishAsync<LanguageChangedEvent, LanguageChangedPayload>(() => new() { LanguageCode = parameter });
    return None.Value;
  }
}
