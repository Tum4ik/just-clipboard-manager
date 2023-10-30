using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.Services;

internal interface ISettingsService : IPluginSettingsService
{
  KeyBindingDescriptor HotkeyShowPasteWindow { get; set; }
  PasteWindowSnappingType PasteWindowSnappingType { get; set; }
}


internal enum PasteWindowSnappingType
{
  Mouse, Caret, DisplayCorner
}
