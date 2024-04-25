using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IKeyboardHookService : IHookService
{
  bool RegisterShowPasteWindowHotkey(KeyBindingDescriptor? descriptor = null);
  void UnregisterHotKey(KeyBindingDescriptor descriptor);
  void UnregisterAll();
}
