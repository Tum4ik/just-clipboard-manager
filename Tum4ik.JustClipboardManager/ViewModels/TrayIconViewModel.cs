using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Castle.Core.Internal;
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


  private TaskCompletionSource<ICollection<FormattedDataObject>>? _tcs;


  private async Task HandleInsertHotKey()
  {
    var targetWindowToPaste = GetForegroundWindow();
    _tcs = new();
    _eventAggregator
      .GetEvent<PasteWindowResultEvent>()
      .Subscribe(HandlePasteWindowResult, ThreadOption.BackgroundThread);
    _pasteWindowService.ShowWindow(targetWindowToPaste);

    var data = await _tcs.Task;
    _tcs = null;
    if (!data.IsNullOrEmpty())
    {
      _pasteService.PasteData(targetWindowToPaste, data);
    }

    _pasteWindowService.HideWindow();
  }


  private void HandlePasteWindowResult(ICollection<FormattedDataObject> formattedDataObjects)
  {
    _eventAggregator.GetEvent<PasteWindowResultEvent>().Unsubscribe(HandlePasteWindowResult);
    _tcs?.SetResult(formattedDataObjects);
  }


  /// <summary>
  /// Retrieves a handle to the foreground window (the window with which the user is currently working). The system
  /// assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
  /// </summary>
  /// <returns>
  /// C++ ( Type: Type: HWND )<br /> The return value is a handle to the foreground window. The foreground window
  /// can be NULL in certain circumstances, such as when a window is losing activation.
  /// </returns>
  [DllImport("user32.dll")]
  private static extern IntPtr GetForegroundWindow();
}
