using System.Windows.Documents;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main;
internal class AboutViewModel : TranslationViewModel
{
  private readonly IInfoService _infoService;

  public AboutViewModel(ITranslationService translationService,
                        IInfoService infoService)
    : base(translationService)
  {
    _infoService = infoService;
  }


  private string? _version;
  public string Version => _version ??= $"{_infoService.Version} ({(Environment.Is64BitProcess ? "64" : "32")}-bit)";
}
