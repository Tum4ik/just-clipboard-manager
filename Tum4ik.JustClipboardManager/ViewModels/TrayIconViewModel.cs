using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PubSub;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.EventPayloads;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class TrayIconViewModel
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly IPasteWindowService _pasteWindowService;
  private readonly IPasteService _pasteService;
  private readonly ISubscriber _subscriber;

  public TrayIconViewModel(IKeyboardHookService keyboardHookService,
                           IPasteWindowService pasteWindowService,
                           IPasteService pasteService,
                           ISubscriber subscriber)
  {
    _keyboardHookService = keyboardHookService;
    _pasteWindowService = pasteWindowService;
    _pasteService = pasteService;
    _subscriber = subscriber;

    var ctrlShiftV = new KeybindDescriptor(ModifierKeys.Control | ModifierKeys.Shift, Key.V);
    _keyboardHookService.RegisterHotKey(ctrlShiftV, HandleInsertHotKey);
  }


  [ICommand]
  private void Exit()
  {
    Application.Current.Shutdown();
  }


  private void HandleInsertHotKey()
  {
    var targetWindowToPaste = GetForegroundWindow();
    _subscriber.Subscribe<PasteWindowResult>(this, r =>
    {
      _subscriber.Unsubscribe<PasteWindowResult>(this);
      _pasteService.PasteData(targetWindowToPaste, r.Data);
    });
    _pasteWindowService.ShowWindow();
  }


  [DllImport("user32.dll")]
  private static extern IntPtr GetForegroundWindow();
}
