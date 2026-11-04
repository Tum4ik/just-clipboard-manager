using System.ComponentModel.DataAnnotations;

namespace JustClipboardManager.Infrastructure.Configurations;

internal class SettingsFilesOptions
{
  public const string SettingsFiles = nameof(SettingsFiles);

  [Required] public required string UserSettings { get; init; }
  [Required] public required string PluginsSettings { get; init; }
}
