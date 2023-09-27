using System.Globalization;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;

public interface ISettingsService
{
  CultureInfo Language { get; set; }
  string Theme { get; set; }

  KeyBindingDescriptor HotkeyShowPasteWindow { get; set; }
}
