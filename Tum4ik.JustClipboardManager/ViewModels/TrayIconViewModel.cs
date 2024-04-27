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
  private readonly IDialogService _dialogService;
  private readonly IThemeService _themeService;
  private readonly IApplicationLifetime _applicationLifetime;
  private readonly IInfoService _infoService;

  public TrayIconViewModel(IDialogService dialogService,
                           ITranslationService translationService,
                           IThemeService themeService,
                           IEventAggregator eventAggregator,
                           IApplicationLifetime applicationLifetime,
                           IInfoService infoService)
    : base(translationService, eventAggregator)
  {
    _dialogService = dialogService;
    _themeService = themeService;
    _applicationLifetime = applicationLifetime;
    _infoService = infoService;

    eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(() => OnPropertyChanged(nameof(SelectedTheme)));
  }


  public string ProductName => _infoService.ProductName;

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
  private void Exit()
  {
    _applicationLifetime.ExitApplication();
  }
}
