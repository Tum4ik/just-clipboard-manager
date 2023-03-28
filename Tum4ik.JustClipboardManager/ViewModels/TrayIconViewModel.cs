using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
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

    // todo: hotkey may be already registered in the OS, needs to be handled such situation
    _keyboardHookService.RegisterShowPasteWindowHotkey(settingsService.HotkeyShowPasteWindow);
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
