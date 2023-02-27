using System.Globalization;

namespace Tum4ik.JustClipboardManager.Services;

internal interface ISettingsService
{
  CultureInfo Language { get; set; }
  Theme Theme { get; set; }
}
