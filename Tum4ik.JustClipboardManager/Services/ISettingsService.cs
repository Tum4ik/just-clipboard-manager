using System.Globalization;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Services;

internal interface ISettingsService
{
  CultureInfo Language { get; set; }
  string Theme { get; set; }

  KeyBindingDescriptor HotkeyShowPasteWindow { get; set; }
}
