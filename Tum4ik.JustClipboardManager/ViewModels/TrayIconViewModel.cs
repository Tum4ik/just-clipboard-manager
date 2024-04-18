using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Theme;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class TrayIconViewModel : TranslationViewModel
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly Lazy<IDialogService> _dialogService;
  private readonly IThemeService _themeService;
  private readonly Lazy<IApplicationLifetime> _applicationLifetime;

  public TrayIconViewModel(IKeyboardHookService keyboardHookService,
                           Lazy<IDialogService> dialogService,
                           ITranslationService translationService,
                           IThemeService themeService,
                           IEventAggregator eventAggregator,
                           Lazy<IApplicationLifetime> applicationLifetime)
    : base(translationService, eventAggregator)
  {
    _keyboardHookService = keyboardHookService;
    _dialogService = dialogService;
    _themeService = themeService;
    _applicationLifetime = applicationLifetime;

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
    if (!_keyboardHookService.RegisterShowPasteWindowHotkey())
    {
      var parameters = new DialogParameters
      {
        { DialogParameterNames.ViewToShow, ViewNames.SettingsView }
      };
      _dialogService.Value.ShowMainAppDialog(parameters);
      _dialogService.Value.Show(DialogNames.UnregisteredHotkeysDialog);
    }
  }


  [RelayCommand]
  private void OpenMainDialog(string viewName)
  {
    var parameters = new DialogParameters
    {
      { DialogParameterNames.ViewToShow, viewName }
    };
    _dialogService.Value.ShowMainAppDialog(parameters);
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
  private void Exit()
  {
    _applicationLifetime.Value.ExitApplication();
  }
}
