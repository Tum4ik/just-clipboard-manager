using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Exceptions;

namespace Tum4ik.JustClipboardManager.Services;
internal sealed class KeyboardHookService : IKeyboardHookService, IDisposable
{
  public KeyboardHookService(IPasteWindowService pasteWindowService)
  {
    _windowHandle = pasteWindowService.WindowHandle;
  }


  private readonly IntPtr _windowHandle;
  private readonly Dictionary<KeybindDescriptor, int> _registeredAtoms = new();
  private readonly Dictionary<int, Delegate> _registeredActionCallbacks = new();


  public void RegisterHotKey(KeybindDescriptor descriptor, Action action)
  {
    RegisterHotKey(descriptor, (Delegate) action);
  }


  public void RegisterHotKey(KeybindDescriptor descriptor, Func<Task> action)
  {
    RegisterHotKey(descriptor, (Delegate) action);
  }


  public void UnregisterHotKey(KeybindDescriptor descriptor)
  {
    if (_registeredAtoms.TryGetValue(descriptor, out var atom))
    {
      UnregisterHotKey(_windowHandle, atom);
      GlobalDeleteAtom(atom);

      _registeredAtoms.Remove(descriptor);
      _registeredActionCallbacks.Remove(atom);
    }
  }


  public void UnregisterAll()
  {
    foreach (var atom in _registeredAtoms.Values)
    {
      UnregisterHotKey(_windowHandle, atom);
      GlobalDeleteAtom(atom);
    }
    _registeredAtoms.Clear();
    _registeredActionCallbacks.Clear();
  }


  private void RegisterHotKey(KeybindDescriptor descriptor, Delegate action)
  {
    var atom = GlobalAddAtom(descriptor.ToString());
    var modifiers = (int) descriptor.Modifiers;
    var vKey = KeyInterop.VirtualKeyFromKey(descriptor.Key);
    if (RegisterHotKey(_windowHandle, atom, modifiers, vKey))
    {
      _registeredAtoms[descriptor] = atom;
      _registeredActionCallbacks[atom] = action;
    }
    else
    {
      GlobalDeleteAtom(atom);
      throw new HotKeyRegistrationException($"Impossible to register hot key '{descriptor}'.");
    }
  }


  public IntPtr HwndHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
  {
    switch (msg)
    {
      case 0x0312: // WM_HOTKEY
        var atom = wParam.ToInt32();
        if (_registeredActionCallbacks.TryGetValue(atom, out var action))
        {
          action.DynamicInvoke();
        }
        break;
    }
    return IntPtr.Zero;
  }


  public void Dispose()
  {
    UnregisterAll();
  }


  [DllImport("user32.dll")]
  private static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, int vKey);

  [DllImport("user32.dll")]
  private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

  [DllImport("kernel32.dll")]
  private static extern int GlobalAddAtom(string name);

  [DllImport("kernel32.dll")]
  private static extern int GlobalDeleteAtom(int nAtom);
}
