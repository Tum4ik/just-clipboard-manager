using System.Collections.Immutable;

namespace Tum4ik.JustClipboardManager.Services.Translation;

internal interface ITranslationService
{
  string this[string key] { get; }
  ImmutableArray<Language> SupportedLanguages { get; }
  Language SelectedLanguage { get; set; }
}
