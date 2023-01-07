using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels;

[INotifyPropertyChanged]
internal partial class TrayIconViewModel
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly IPasteWindowService _pasteWindowService;
  private readonly IPasteService _pasteService;
  private readonly IEventAggregator _eventAggregator;
  private readonly IThemeService _themeService;
  private readonly IDialogService _dialogService;

  public TrayIconViewModel(IKeyboardHookService keyboardHookService,
                           IPasteWindowService pasteWindowService,
                           IPasteService pasteService,
                           IEventAggregator eventAggregator,
                           IThemeService themeService,
                           IDialogService dialogService)
  {
    _keyboardHookService = keyboardHookService;
    _pasteWindowService = pasteWindowService;
    _pasteService = pasteService;
    _eventAggregator = eventAggregator;
    _themeService = themeService;
    _dialogService = dialogService;

    // todo: from settings
#if DEBUG
    var ctrlShiftV = new KeybindDescriptor(ModifierKeys.Control | ModifierKeys.Alt, Key.V);
#else
    var ctrlShiftV = new KeybindDescriptor(ModifierKeys.Control | ModifierKeys.Shift, Key.V);
#endif
    _keyboardHookService.RegisterHotKey(ctrlShiftV, HandleInsertHotKeyAsync);
  }


  [ObservableProperty]
  private string _trayIcon
#if DEBUG
    = "/Resources/Icons/tray-dev.ico";
#else
    = "/Resources/Icons/tray.ico";
#endif


  [RelayCommand]
  private void Settings()
  {
    _dialogService.Show(DialogNames.MainDialog);
  }


  [RelayCommand]
  private void ChangeTheme(Theme theme)
  {
    _themeService.SetTheme(theme);
  }


  [RelayCommand]
  private static void Exit()
  {
    Application.Current.Shutdown();
  }


  private TaskCompletionSource<ICollection<FormattedDataObject>>? _tcs;


  private async Task HandleInsertHotKeyAsync()
  {
    var targetWindowToPaste = GetForegroundWindow();
    _tcs = new();
    _eventAggregator
      .GetEvent<PasteWindowResultEvent>()
      .Subscribe(HandlePasteWindowResult, ThreadOption.BackgroundThread);
    _pasteWindowService.ShowWindow(targetWindowToPaste);

    var data = await _tcs.Task.ConfigureAwait(true);
    _tcs = null;
    if (data.Count > 0)
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
  [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
  private static extern nint GetForegroundWindow();
}
