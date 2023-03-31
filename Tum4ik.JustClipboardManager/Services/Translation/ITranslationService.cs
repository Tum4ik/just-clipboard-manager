namespace Tum4ik.JustClipboardManager.Services.Translation;

internal interface ITranslationService
{
  string this[string key] { get; }
  IEnumerable<Language> SupportedLanguages { get; }
  Language SelectedLanguage { get; set; }
  event Action? LanguageChanged;
}
