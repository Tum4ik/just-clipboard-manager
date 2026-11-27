using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.IPC.Commands;
using JustClipboardManager.Application.IPC.Common;
using JustClipboardManager.Application.IPC.Events.Payloads;
using JustClipboardManager.Application.Services;

namespace JustClipboardManager.Application.Commands.Settings.Interface;

internal class SetSelectedLanguageCommand : AppCommand<SetSelectedLanguageParameter, Empty>
{
  private readonly ISettingsService _settingsService;
  private readonly IEventsPublisher _eventsPublisher;

  public SetSelectedLanguageCommand(ISettingsService settingsService,
                                    IEventsPublisher eventsPublisher)
  {
    _settingsService = settingsService;
    _eventsPublisher = eventsPublisher;
  }

  protected override async Task<Empty> ExecuteAsync(SetSelectedLanguageParameter parameter,
                                                    CancellationToken cancellationToken = default)
  {
    _settingsService.UserSettings.SelectedLanguageCode = parameter.LanguageCode;
    await _settingsService.SaveChangesAsync();
    await _eventsPublisher.PublishAsync<LanguageChangedPayload>(payload => payload.LanguageCode = parameter.LanguageCode);
    return new();
  }
}
