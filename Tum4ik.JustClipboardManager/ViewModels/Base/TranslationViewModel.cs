using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Navigation;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.ViewModels.Base;

internal abstract class TranslationViewModel : ObservableObject, IDestructible
{
  public ITranslationService Translate { get; }

  protected TranslationViewModel(ITranslationService translationService)
  {
    Translate = translationService;

    translationService.LanguageChanged += LanguageChanged;
  }


  public virtual void Destroy()
  {
    Translate.LanguageChanged -= LanguageChanged;
  }


  private void LanguageChanged()
  {
    OnPropertyChanged(nameof(Translate));
  }
}
