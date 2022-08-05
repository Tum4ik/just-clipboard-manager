using System;
using System.Threading.Tasks;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IKeyboardHookService : IHookService
{
  void RegisterHotKey(KeybindDescriptor descriptor, Action action);
  void RegisterHotKey(KeybindDescriptor descriptor, Func<Task> action);
  void UnregisterHotKey(KeybindDescriptor descriptor);
  void UnregisterAll();
}
