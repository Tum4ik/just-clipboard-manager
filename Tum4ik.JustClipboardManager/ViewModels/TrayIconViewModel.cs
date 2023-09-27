using System.Collections.Immutable;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Theme;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class TrayIconViewModel : TranslationViewModel
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly IDialogService _dialogService;
  private readonly IThemeService _themeService;
  private readonly ISettingsService _settingsService;

  public TrayIconViewModel(IKeyboardHookService keyboardHookService,
                           IDialogService dialogService,
                           ITranslationService translationService,
                           IThemeService themeService,
                           ISettingsService settingsService,
                           IEventAggregator eventAggregator)
    : base(translationService, eventAggregator)
  {
    _keyboardHookService = keyboardHookService;
    _dialogService = dialogService;
    _themeService = themeService;
    _settingsService = settingsService;

    SetupHotkeys();
    eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(() => OnPropertyChanged(nameof(SelectedTheme)));
  }


  [ObservableProperty]
  private string _trayIcon
#if DEBUG
    = "/Resources/Icons/tray-dev.ico";
#else
    = "/Resources/Icons/tray.ico";
#endif


  public ImmutableArray<ColorTheme> Themes => _themeService.Themes;


  public ColorTheme SelectedTheme
  {
    get => _themeService.SelectedTheme;
    set => _themeService.SelectedTheme = value;
  }


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
    SelectedTheme = theme;
  }


  [RelayCommand]
  private void ChangeLanguage(Language language)
  {
    Translate.SelectedLanguage = language;
  }


  [RelayCommand]
  private static void Exit()
  {
    Application.Current.Shutdown();
  }
}
