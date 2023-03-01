using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Events;
using Prism.Navigation;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.ViewModels.Base;

internal abstract class TranslationViewModel : ObservableObject, IDestructible
{
  public ITranslationService Translate { get; }
  protected readonly IEventAggregator EventAggregator;

  protected TranslationViewModel(ITranslationService translationService,
                                 IEventAggregator eventAggregator)
  {
    Translate = translationService;
    EventAggregator = eventAggregator;

    EventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(LanguageChanged);
  }


  public virtual void Destroy()
  {
    EventAggregator.GetEvent<LanguageChangedEvent>().Unsubscribe(LanguageChanged);
  }


  private void LanguageChanged()
  {
    OnPropertyChanged(nameof(Translate));
  }
}
