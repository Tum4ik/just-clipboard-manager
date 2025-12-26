using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main;
internal partial class AboutViewModel : TranslationViewModel
{
  private readonly IInfoService _infoService;

  public AboutViewModel(ITranslationService translationService,
                        IEventAggregator eventAggregator,
                        IInfoService infoService)
    : base(translationService, eventAggregator)
  {
    _infoService = infoService;
  }


  private string? _version;
  public string Version => _version ??= $"{_infoService.Version} ({(Environment.Is64BitProcess ? "64" : "32")}-bit)";

  public string Email { get; } = "timchishinevgeniy@gmail.com";


  [RelayCommand]
  private void OpenLink(string? link)
  {
    if (string.IsNullOrWhiteSpace(link))
    {
      return;
    }
    Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
  }


  [RelayCommand]
  private void CopyEmailToClipboard()
  {
    Clipboard.SetText(Email);
  }
}
