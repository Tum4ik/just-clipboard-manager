namespace JustClipboardManager.Application.Services;

public interface ISettingsService
{
  UserSettings UserSettings { get; }

  Task LoadSettingsAsync();
  Task SaveChangesAsync();
}


public class UserSettings
{
  public string SelectedLanguageCode { get; set; } = "en";
}
