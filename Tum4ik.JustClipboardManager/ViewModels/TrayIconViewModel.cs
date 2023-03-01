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
using Tum4ik.JustClipboardManager.Services.PInvoke;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class TrayIconViewModel : TranslationSelectionViewModel
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly IPasteWindowService _pasteWindowService;
  private readonly IPasteService _pasteService;
  private readonly IThemeService _themeService;
  private readonly IDialogService _dialogService;
  private readonly IUser32DllService _user32Dll;

  public TrayIconViewModel(IKeyboardHookService keyboardHookService,
                           IPasteWindowService pasteWindowService,
                           IPasteService pasteService,
                           IEventAggregator eventAggregator,
                           IThemeService themeService,
                           IDialogService dialogService,
                           ITranslationService translationService,
                           IUser32DllService user32Dll,
                           ISettingsService settingsService)
    : base(translationService, eventAggregator, settingsService)
  {
    _keyboardHookService = keyboardHookService;
    _pasteWindowService = pasteWindowService;
    _pasteService = pasteService;
    _themeService = themeService;
    _dialogService = dialogService;
    _user32Dll = user32Dll;

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
  private void OpenMainDialog(string viewName)
  {
    var parameters = new DialogParameters
    {
      { DialogParameterNames.ViewToShow, viewName }
    };
    _dialogService.Show(DialogNames.MainDialog, parameters, r => { });
  }


  [RelayCommand]
  private void ChangeTheme(Theme theme)
  {
    _themeService.SetTheme(theme);
  }


  [RelayCommand]
  private void ChangeLanguage(Language language)
  {
    SelectedLanguage = language;
    // Important to trigger SelectedLanguage changed to keep it checked on the UI side
    // in case the SelectedLanguage property value is not changed.
    OnPropertyChanged(nameof(SelectedLanguage));
  }


  [RelayCommand]
  private static void Exit()
  {
    Application.Current.Shutdown();
  }


  private TaskCompletionSource<ICollection<FormattedDataObject>>? _tcs;


  private async Task HandleInsertHotKeyAsync()
  {
    var targetWindowToPaste = _user32Dll.GetForegroundWindow();
    _tcs = new();
    EventAggregator
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
    EventAggregator.GetEvent<PasteWindowResultEvent>().Unsubscribe(HandlePasteWindowResult);
    _tcs?.SetResult(formattedDataObjects);
  }
}
