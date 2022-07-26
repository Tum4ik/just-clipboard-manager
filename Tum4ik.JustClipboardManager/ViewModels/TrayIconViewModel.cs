using System.Runtime.InteropServices;
using System;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Tum4ik.JustClipboardManager.Models;
using Tum4ik.JustClipboardManager.Services;
using System.Threading.Tasks;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class TrayIconViewModel
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly IPasteWindowService _pasteWindowService;
  private readonly IPasteService _pasteService;

  public TrayIconViewModel(IKeyboardHookService keyboardHookService,
                           IPasteWindowService pasteWindowService,
                           IPasteService pasteService)
  {
    _keyboardHookService = keyboardHookService;
    _pasteWindowService = pasteWindowService;
    _pasteService = pasteService;
    _keyboardHookService.Start(_pasteWindowService.GetWindowHandle());
    var ctrlShiftV = new KeybindDescriptor(ModifierKeys.Control | ModifierKeys.Shift, Key.V);
    _keyboardHookService.RegisterHotKey(ctrlShiftV, HandleInsertHotKey);
  }


  [ICommand]
  private void Exit()
  {
    _keyboardHookService.Stop();
    Application.Current.Shutdown();
  }


  private async Task HandleInsertHotKey()
  {
    var targetWindowToPaste = GetForegroundWindow();
    var result = await _pasteWindowService.ShowWindowAsync(targetWindowToPaste);
    _pasteService.PasteData(targetWindowToPaste, result?.Data);
  }


  [DllImport("user32.dll")]
  private static extern IntPtr GetForegroundWindow();
}
