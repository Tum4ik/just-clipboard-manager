using Prism.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal class SettingsInterfaceViewModel : TranslationSelectionViewModel
{
  public SettingsInterfaceViewModel(ITranslationService translationService,
                                    IEventAggregator eventAggregator,
                                    ISettingsService settingsService)
    : base(translationService, eventAggregator, settingsService)
  {
  }
}
