using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Tum4ik.JustClipboardManager.Models;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class TrayIconViewModel
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly IPasteWindowService _pasteWindowService;

  public TrayIconViewModel(IKeyboardHookService keyboardHookService,
                           IPasteWindowService pasteWindowService)
  {
    _keyboardHookService = keyboardHookService;
    _pasteWindowService = pasteWindowService;

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


  private void HandleInsertHotKey()
  {
    _pasteWindowService.ShowWindow();
  }
}
