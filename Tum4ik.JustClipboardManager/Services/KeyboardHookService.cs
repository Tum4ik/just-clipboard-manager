using System.Windows.Input;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using static Windows.Win32.PInvoke;

namespace Tum4ik.JustClipboardManager.Services;
internal sealed class KeyboardHookService : IKeyboardHookService, IDisposable
{
  private readonly IPasteWindowService _pasteWindowService;
  private readonly IUser32DllService _user32Dll;
  private readonly IKernel32DllService _kernel32Dll;
  private readonly IPasteService _pasteService;
  private readonly ISettingsService _settingsService;

  public KeyboardHookService(IPasteWindowService pasteWindowService,
                             IUser32DllService user32Dll,
                             IKernel32DllService kernel32Dll,
                             IPasteService pasteService,
                             ISettingsService settingsService)
  {
    _windowHandle = pasteWindowService.WindowHandle;
    _pasteWindowService = pasteWindowService;
    _user32Dll = user32Dll;
    _kernel32Dll = kernel32Dll;
    _pasteService = pasteService;
    _settingsService = settingsService;
  }


  private readonly nint _windowHandle;
  private readonly Dictionary<KeyBindingDescriptor, ushort> _registeredAtoms = new();
  private readonly Dictionary<int, Delegate> _registeredActionCallbacks = new();


  public bool RegisterShowPasteWindowHotkey(KeyBindingDescriptor? descriptor = null)
  {
    descriptor ??= _settingsService.HotkeyShowPasteWindow;
    var success = RegisterHotKey(
      descriptor, HandleShowPasteWindowHotkeyAsync, () => _settingsService.HotkeyShowPasteWindow = descriptor
    );
    if (!success)
    {
      _settingsService.HotkeyShowPasteWindow = new(ModifierKeys.None, Key.None);
    }
    return success;
  }


  public void UnregisterHotKey(KeyBindingDescriptor descriptor)
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


  private bool RegisterHotKey(KeyBindingDescriptor descriptor, Delegate action, Action saveToSettings)
  {
    var atom = _kernel32Dll.GlobalAddAtom(descriptor.ToString());
    if (_user32Dll.RegisterHotKey(_windowHandle, atom, descriptor.Modifiers, descriptor.Key))
    {
      _registeredAtoms[descriptor] = atom;
      _registeredActionCallbacks[atom] = action;
      saveToSettings();
      return true;
    }

    _kernel32Dll.GlobalDeleteAtom(atom);
    return false;
  }


  public nint HwndHook(nint hWnd, int msg, nint wParam, nint lParam, ref bool handled)
  {
    switch ((uint) msg)
    {
      case WM_HOTKEY:
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

  
  private bool _isWaitingPasteWindowResult;

  private async Task HandleShowPasteWindowHotkeyAsync()
  {
    if (_isWaitingPasteWindowResult)
    {
      return;
    }
    _isWaitingPasteWindowResult = true;

    var targetWindowToPaste = _user32Dll.GetForegroundWindow();
    var data = await _pasteWindowService.ShowWindowAsync(targetWindowToPaste).ConfigureAwait(true);
    if (data is not null && data.FormattedDataObjects.Count > 0)
    {
      _pasteService.PasteData(targetWindowToPaste, data.FormattedDataObjects, data.AdditionalInfo);
    }

    _pasteWindowService.HideWindow();
    
    _isWaitingPasteWindowResult = false;
  }
}
