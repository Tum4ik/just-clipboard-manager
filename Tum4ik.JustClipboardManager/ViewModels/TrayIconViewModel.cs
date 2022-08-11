using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Tum4ik.EventAggregator;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class TrayIconViewModel
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly IPasteWindowService _pasteWindowService;
  private readonly IPasteService _pasteService;
  private readonly IEventAggregator _eventAggregator;

  public TrayIconViewModel(IKeyboardHookService keyboardHookService,
                           IPasteWindowService pasteWindowService,
                           IPasteService pasteService,
                           IEventAggregator eventAggregator)
  {
    _keyboardHookService = keyboardHookService;
    _pasteWindowService = pasteWindowService;
    _pasteService = pasteService;
    _eventAggregator = eventAggregator;

    var ctrlShiftV = new KeybindDescriptor(ModifierKeys.Control | ModifierKeys.Shift, Key.V);
    _keyboardHookService.RegisterHotKey(ctrlShiftV, HandleInsertHotKey);
  }


  [RelayCommand]
  private void Exit()
  {
    Application.Current.Shutdown();
  }


  private TaskCompletionSource<string>? _tcs;


  private async Task HandleInsertHotKey()
  {
    var targetWindowToPaste = GetForegroundWindow();
    _tcs = new();
    _eventAggregator
      .GetEvent<PasteWindowResultEvent>()
      .Subscribe(HandlePasteWindowResult, ThreadOption.BackgroundThread);
    _pasteWindowService.ShowWindow();

    var data = await _tcs.Task;
    _tcs = null;
    _pasteService.PasteData(targetWindowToPaste, data);
  }


  private void HandlePasteWindowResult(string result)
  {
    _eventAggregator.GetEvent<PasteWindowResultEvent>().Unsubscribe(HandlePasteWindowResult);
    _tcs?.SetResult(result);
  }


  [DllImport("user32.dll")]
  private static extern IntPtr GetForegroundWindow();
}
