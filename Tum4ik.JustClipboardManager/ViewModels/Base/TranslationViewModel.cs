using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Events;
using Prism.Navigation;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.ViewModels.Base;

internal abstract class TranslationViewModel : ObservableObject, IDestructible
{
  public ITranslationService Translate { get; }
  private readonly IEventAggregator _eventAggregator;

  protected TranslationViewModel(ITranslationService translationService,
                                 IEventAggregator eventAggregator)
  {
    Translate = translationService;
    _eventAggregator = eventAggregator;

    _eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(LanguageChanged);
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
