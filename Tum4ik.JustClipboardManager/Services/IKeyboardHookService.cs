using System;
using System.Threading.Tasks;
using Tum4ik.JustClipboardManager.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IKeyboardHookService
{
  void Start(IntPtr windowHandle);
  void Stop();
  void RegisterHotKey(KeybindDescriptor descriptor, Action action);
  void RegisterHotKey(KeybindDescriptor descriptor, Func<Task> action);
  void UnregisterHotKey(KeybindDescriptor descriptor);
  void UnregisterAll();
}
