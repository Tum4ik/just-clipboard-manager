using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Application.IPC.Commands;
using JustClipboardManager.Application.IPC.Common;
using JustClipboardManager.Application.Services;

namespace JustClipboardManager.Application.Commands.Settings.Interface;

internal class GetSelectedLanguageCommand : AppCommand<Empty, GetSelectedLanguageResult>
{
  private readonly ISettingsService _settingsService;

  public GetSelectedLanguageCommand(ISettingsService settingsService)
  {
    _settingsService = settingsService;
  }

  protected override Task<GetSelectedLanguageResult> ExecuteAsync(Empty parameter,
                                                                  CancellationToken cancellationToken = default)
  {
    var languageCode = _settingsService.UserSettings.SelectedLanguageCode;
    return Task.FromResult<GetSelectedLanguageResult>(new() { LanguageCode = languageCode });
  }
}
