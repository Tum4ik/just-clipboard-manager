using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Theme;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class TrayIconViewModel : TranslationViewModel
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly IDialogService _dialogService;
  private readonly ISettingsService _settingsService;

  public IThemeService ThemeService { get; }

  public TrayIconViewModel(IKeyboardHookService keyboardHookService,
                           IDialogService dialogService,
                           ITranslationService translationService,
                           IThemeService themeService,
                           ISettingsService settingsService)
    : base(translationService)
  {
    _keyboardHookService = keyboardHookService;
    _dialogService = dialogService;
    ThemeService = themeService;
    _settingsService = settingsService;

    SetupHotkeys();
  }


  [ObservableProperty]
  private string _trayIcon
#if DEBUG
    = "/Resources/Icons/tray-dev.ico";
#else
    = "/Resources/Icons/tray.ico";
#endif


  private void SetupHotkeys()
  {
    var emptyDescriptor = new KeyBindingDescriptor(ModifierKeys.None, Key.None);
    if (_settingsService.HotkeyShowPasteWindow == emptyDescriptor
      || !_keyboardHookService.RegisterShowPasteWindowHotkey(_settingsService.HotkeyShowPasteWindow))
    {
      _settingsService.HotkeyShowPasteWindow = emptyDescriptor;
      var parameters = new DialogParameters
      {
        { DialogParameterNames.ViewToShow, ViewNames.SettingsView }
      };
      _dialogService.ShowMainAppDialog(parameters);
      _dialogService.Show(DialogNames.UnregisteredHotkeysDialog);
    }
  }


  [RelayCommand]
  private void OpenMainDialog(string viewName)
  {
    var parameters = new DialogParameters
    {
      { DialogParameterNames.ViewToShow, viewName }
    };
    _dialogService.ShowMainAppDialog(parameters);
  }


  [RelayCommand]
  private void ChangeTheme(ColorTheme theme)
  {
    ThemeService.SelectedTheme = theme;
    OnPropertyChanged(nameof(ThemeService));
  }


  [RelayCommand]
  private void ChangeLanguage(Language language)
  {
    Translate.SelectedLanguage = language;
    // Important to trigger SelectedLanguage changed to keep it checked on the UI side
    // in case the SelectedLanguage property value is not changed.
    OnPropertyChanged(nameof(Translate));
  }


  [RelayCommand]
  private static void Exit()
  {
    Application.Current.Shutdown();
  }
}
