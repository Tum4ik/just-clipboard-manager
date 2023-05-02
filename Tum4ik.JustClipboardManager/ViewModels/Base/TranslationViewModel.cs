using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Events;
using Prism.Navigation;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.ViewModels.Base;

internal abstract class TranslationViewModel : ObservableObject, IDestructible
{
  private readonly IEventAggregator _eventAggregator;

  public ITranslationService Translate { get; }

  protected TranslationViewModel(ITranslationService translationService,
                                 IEventAggregator eventAggregator)
  {
    Translate = translationService;
    _eventAggregator = eventAggregator;

    eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(LanguageChanged);
  }


  public virtual void Destroy()
  {
    _eventAggregator.GetEvent<LanguageChangedEvent>().Unsubscribe(LanguageChanged);
  }


  private void LanguageChanged()
  {
    OnPropertyChanged(nameof(Translate));
  }
}
