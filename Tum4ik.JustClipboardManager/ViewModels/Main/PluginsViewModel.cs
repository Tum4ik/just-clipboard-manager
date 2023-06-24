using Prism.Events;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main;
internal class PluginsViewModel : TranslationViewModel
{
  public PluginsViewModel(ITranslationService translationService,
                          IEventAggregator eventAggregator)
    : base(translationService, eventAggregator)
  {
  }
}
