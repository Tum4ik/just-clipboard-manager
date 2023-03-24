using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Exceptions;
using Tum4ik.JustClipboardManager.Services.PInvoke;

namespace Tum4ik.JustClipboardManager.Services;
internal sealed class KeyboardHookService : IKeyboardHookService, IDisposable
{
  private readonly IUser32DllService _user32Dll;
  private readonly IKernel32DllService _kernel32Dll;

  public KeyboardHookService(IPasteWindowService pasteWindowService,
                             IUser32DllService user32Dll,
                             IKernel32DllService kernel32Dll)
  {
    _windowHandle = pasteWindowService.WindowHandle;
    _user32Dll = user32Dll;
    _kernel32Dll = kernel32Dll;
  }


  private readonly nint _windowHandle;
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
      _user32Dll.UnregisterHotKey(_windowHandle, atom);
      _kernel32Dll.GlobalDeleteAtom(atom);

      _registeredAtoms.Remove(descriptor);
      _registeredActionCallbacks.Remove(atom);
    }
  }


  public void UnregisterAll()
  {
    foreach (var atom in _registeredAtoms.Values)
    {
      _user32Dll.UnregisterHotKey(_windowHandle, atom);
      _kernel32Dll.GlobalDeleteAtom(atom);
    }
    _registeredAtoms.Clear();
    _registeredActionCallbacks.Clear();
  }


  private void RegisterHotKey(KeybindDescriptor descriptor, Delegate action)
  {
    var atom = _kernel32Dll.GlobalAddAtom(descriptor.ToString());
    var modifiers = (int) descriptor.Modifiers;
    var vKey = KeyInterop.VirtualKeyFromKey(descriptor.Key);
    if (_user32Dll.RegisterHotKey(_windowHandle, atom, modifiers, vKey))
    {
      _registeredAtoms[descriptor] = atom;
      _registeredActionCallbacks[atom] = action;
    }
    else
    {
      _kernel32Dll.GlobalDeleteAtom(atom);
      // todo: notify user the hot key is already registered, suggest to choose another hot key
      throw new HotKeyRegistrationException($"Impossible to register hot key '{descriptor}'.");
    }
  }


  public nint HwndHook(nint hWnd, int msg, nint wParam, nint lParam, ref bool handled)
  {
    switch (msg)
    {
      case 0x0312: // WM_HOTKEY
        var atom = wParam.ToInt32();
        if (_registeredActionCallbacks.TryGetValue(atom, out var @delegate))
        {
          switch (@delegate)
          {
            case Action action:
              action();
              break;
            case Func<Task> func:
              func().Await(e => throw e);
              break;
          }
        }
        break;
    }
    return nint.Zero;
  }


  public void Dispose()
  {
    UnregisterAll();
  }
}
