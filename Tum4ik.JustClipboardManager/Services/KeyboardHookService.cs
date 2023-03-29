using System.Windows.Input;
using Prism.Events;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.PInvoke;

namespace Tum4ik.JustClipboardManager.Services;
internal sealed class KeyboardHookService : IKeyboardHookService, IDisposable
{
  private readonly IPasteWindowService _pasteWindowService;
  private readonly IUser32DllService _user32Dll;
  private readonly IKernel32DllService _kernel32Dll;
  private readonly IPasteService _pasteService;
  private readonly IEventAggregator _eventAggregator;
  private readonly ISettingsService _settingsService;

  public KeyboardHookService(IPasteWindowService pasteWindowService,
                             IUser32DllService user32Dll,
                             IKernel32DllService kernel32Dll,
                             IPasteService pasteService,
                             IEventAggregator eventAggregator,
                             ISettingsService settingsService)
  {
    _windowHandle = pasteWindowService.WindowHandle;
    _pasteWindowService = pasteWindowService;
    _user32Dll = user32Dll;
    _kernel32Dll = kernel32Dll;
    _pasteService = pasteService;
    _eventAggregator = eventAggregator;
    _settingsService = settingsService;
  }


  private readonly nint _windowHandle;
  private readonly Dictionary<KeyBindingDescriptor, int> _registeredAtoms = new();
  private readonly Dictionary<int, Delegate> _registeredActionCallbacks = new();


  public bool RegisterShowPasteWindowHotkey(KeyBindingDescriptor descriptor)
  {
    return RegisterHotKey(
      descriptor, HandleShowPasteWindowHotkeyAsync, () => _settingsService.HotkeyShowPasteWindow = descriptor
    );
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
    var modifiers = (int) descriptor.Modifiers;
    var vKey = KeyInterop.VirtualKeyFromKey(descriptor.Key);
    if (_user32Dll.RegisterHotKey(_windowHandle, atom, modifiers, vKey))
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


  private TaskCompletionSource<ICollection<FormattedDataObject>>? _showPasteWindowTcs;

  private async Task HandleShowPasteWindowHotkeyAsync()
  {
    var targetWindowToPaste = _user32Dll.GetForegroundWindow();
    _showPasteWindowTcs = new();
    _eventAggregator
      .GetEvent<PasteWindowResultEvent>()
      .Subscribe(HandlePasteWindowResult, ThreadOption.BackgroundThread);
    _pasteWindowService.ShowWindow(targetWindowToPaste);

    var data = await _showPasteWindowTcs.Task.ConfigureAwait(true);
    _showPasteWindowTcs = null;
    if (data.Count > 0)
    {
      _pasteService.PasteData(targetWindowToPaste, data);
    }

    _pasteWindowService.HideWindow();
  }

  private void HandlePasteWindowResult(ICollection<FormattedDataObject> formattedDataObjects)
  {
    _eventAggregator.GetEvent<PasteWindowResultEvent>().Unsubscribe(HandlePasteWindowResult);
    _showPasteWindowTcs?.SetResult(formattedDataObjects);
  }
}
