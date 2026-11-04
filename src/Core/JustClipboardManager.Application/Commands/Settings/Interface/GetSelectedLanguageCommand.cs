using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.Services;
using JustClipboardManager.Domain.Common;

namespace JustClipboardManager.Application.Commands.Settings.Interface;

internal class GetSelectedLanguageCommand : AppCommand<None, string>
{
  private readonly ISettingsService _settingsService;

  public GetSelectedLanguageCommand(ISettingsService settingsService)
  {
    _settingsService = settingsService;
  }

  protected override Task<string> ExecuteInternalAsync(None parameter, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(_settingsService.UserSettings.SelectedLanguageCode);
  }
}
