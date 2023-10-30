using System.Globalization;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;
public interface IPluginSettingsService
{
  CultureInfo Language { get; set; }
  string Theme { get; set; }
}
