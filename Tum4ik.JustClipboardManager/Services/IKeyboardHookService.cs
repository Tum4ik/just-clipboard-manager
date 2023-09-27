using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IKeyboardHookService : IHookService
{
  bool RegisterShowPasteWindowHotkey(KeyBindingDescriptor descriptor);
  void UnregisterHotKey(KeyBindingDescriptor descriptor);
  void UnregisterAll();
}
