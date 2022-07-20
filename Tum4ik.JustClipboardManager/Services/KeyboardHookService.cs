using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using Tum4ik.JustClipboardManager.Exceptions;
using Tum4ik.JustClipboardManager.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal class KeyboardHookService : IKeyboardHookService
{
  private IntPtr _windowHandle;
  private HwndSource? _hwndSource;
  private bool _isStarted;
  private readonly Dictionary<KeybindDescriptor, int> _registeredAtoms = new();
  private readonly Dictionary<int, Action> _registeredCallbacks = new();


  public void Start(IntPtr windowHandle)
  {
    if (_isStarted)
    {
      return;
    }

    _windowHandle = windowHandle;
    _hwndSource = HwndSource.FromHwnd(_windowHandle);
    _hwndSource.AddHook(HwndHook);
    _isStarted = true;
  }


  public void Stop()
  {
    if (!_isStarted)
    {
      return;
    }

    UnregisterAll();
    _hwndSource?.RemoveHook(HwndHook);
    _isStarted = false;
  }


  public void RegisterHotKey(KeybindDescriptor descriptor, Action action)
  {
    var atom = GlobalAddAtom(descriptor.ToString());
    var modifiers = (int) descriptor.Modifiers;
    var vKey = KeyInterop.VirtualKeyFromKey(descriptor.Key);
    if (RegisterHotKey(_windowHandle, atom, modifiers, vKey))
    {
      _registeredAtoms[descriptor] = atom;
      _registeredCallbacks[atom] = action;
    }
    else
    {
      GlobalDeleteAtom(atom);
      throw new HotKeyRegistrationException($"Impossible to register hot key '{descriptor}'.");
    }
  }


  public void UnregisterHotKey(KeybindDescriptor descriptor)
  {
    if (_registeredAtoms.TryGetValue(descriptor, out var atom))
    {
      UnregisterHotKey(_windowHandle, atom);
      GlobalDeleteAtom(atom);

      _registeredAtoms.Remove(descriptor);
      _registeredCallbacks.Remove(atom);
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
    _registeredCallbacks.Clear();
  }


  private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
  {
    switch (msg)
    {
      case 0x0312: // WM_HOTKEY
        var atom = wParam.ToInt32();
        if (_registeredCallbacks.TryGetValue(atom, out var action))
        {
          action();
          handled = true;
        }
        break;
    }
    return IntPtr.Zero;
  }


  [DllImport("user32.dll")]
  private static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, int vKey);

  [DllImport("user32.dll")]
  private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

  [DllImport("kernel32.dll")]
  public static extern int GlobalAddAtom(string name);

  [DllImport("kernel32.dll")]
  public static extern int GlobalDeleteAtom(int nAtom);
}
