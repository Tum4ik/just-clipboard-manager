using Prism.Events;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal class SettingsPasteWindowViewModel : TranslationViewModel
{
  public SettingsPasteWindowViewModel(ITranslationService translationService,
                                      IEventAggregator eventAggregator)
    : base(translationService, eventAggregator)
  {
  }
}
