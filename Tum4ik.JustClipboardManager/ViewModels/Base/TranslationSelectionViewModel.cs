using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.ViewModels.Base;

internal abstract partial class TranslationSelectionViewModel : TranslationViewModel
{
  protected readonly ISettingsService SettingsService;

  protected TranslationSelectionViewModel(ITranslationService translationService,
                                          IEventAggregator eventAggregator,
                                          ISettingsService settingsService)
    : base(translationService, eventAggregator)
  {
    SettingsService = settingsService;

    Languages = translationService.SupportedLanguages;
    SelectedLanguage = GetLanguageFromSettings();
    eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(LanguageChanged);
  }

  
  public Language[] Languages { get; }


  [ObservableProperty] private Language? _selectedLanguage;
  partial void OnSelectedLanguageChanged(Language? value)
  {
    if (value is not null)
    {
      // Because we don't want to handle the LanguageChangedEvent we produce ourselves, we have to unsubscribe
      // and subscribe after action again.
      EventAggregator.GetEvent<LanguageChangedEvent>().Unsubscribe(LanguageChanged);
      SettingsService.Language = value.Culture;
      EventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(LanguageChanged);
    }
  }


  public override void Destroy()
  {
    base.Destroy();
    EventAggregator.GetEvent<LanguageChangedEvent>().Unsubscribe(LanguageChanged);
  }


  private void LanguageChanged()
  {
    // We need this handler to detect the language change outside this class (without using SelectedLanguage property)
    // and properly updated the SelectedLanguage property.
    SelectedLanguage = GetLanguageFromSettings();
  }


  private Language? GetLanguageFromSettings()
  {
    return Languages.SingleOrDefault(l => l.Culture.Equals(SettingsService.Language));
  }
}
