namespace Tum4ik.JustClipboardManager.Services.Translation;

internal interface ITranslationService
{
  string this[string key] { get; }
  Language[] SupportedLanguages { get; }
}
