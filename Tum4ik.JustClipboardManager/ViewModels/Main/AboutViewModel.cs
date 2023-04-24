using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main;
internal partial class AboutViewModel : TranslationViewModel
{
  private readonly IInfoService _infoService;
  private readonly IEnvironment _environment;
  private readonly IProcess _process;
  private readonly IClipboard _clipboard;

  public AboutViewModel(ITranslationService translationService,
                        IInfoService infoService,
                        IEnvironment environment,
                        IProcess process,
                        IClipboard clipboard)
    : base(translationService)
  {
    _infoService = infoService;
    _environment = environment;
    _process = process;
    _clipboard = clipboard;
  }


  private string? _version;
  public string Version => _version ??= $"{_infoService.Version} ({(_environment.Is64BitProcess ? "64" : "32")}-bit)";

  public string Email { get; } = "timchishinevgeniy@gmail.com";


  [RelayCommand]
  private void OpenLink(string? link)
  {
    if (string.IsNullOrWhiteSpace(link))
    {
      return;
    }
    _process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
  }


  [RelayCommand]
  private void CopyEmailToClipboard()
  {
    _clipboard.SetText(Email);
  }
}
