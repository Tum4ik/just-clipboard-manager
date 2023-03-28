using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IKeyboardHookService : IHookService
{
  void RegisterShowPasteWindowHotkey(KeyBindingDescriptor descriptor);
  void UnregisterHotKey(KeyBindingDescriptor descriptor);
  void UnregisterAll();
}
