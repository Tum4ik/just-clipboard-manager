using System.Collections.Immutable;
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
  private readonly IAppEnvironmentService _appEnvironmentService;

  public TrayIconViewModel(IDialogService dialogService,
                           ITranslationService translationService,
                           IThemeService themeService,
                           IEventAggregator eventAggregator,
                           IApplicationLifetime applicationLifetime,
                           IInfoService infoService,
                           IAppEnvironmentService appEnvironmentService)
    : base(translationService, eventAggregator)
  {
    _dialogService = dialogService;
    _themeService = themeService;
    _applicationLifetime = applicationLifetime;
    _infoService = infoService;
    _appEnvironmentService = appEnvironmentService;
    eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(() => OnPropertyChanged(nameof(SelectedTheme)));
  }


  private string? _productName;
  public string ProductName => _productName ??= _infoService.ProductName + _appEnvironmentService.Environment switch
  {
    AppEnvironment.Production => string.Empty,
    AppEnvironment.Development => " - Development",
    AppEnvironment.UiTest => " - UI Test",
    _ => string.Empty
  };

  public string TrayIcon
  {
    get
    {
      if (_appEnvironmentService.Environment == AppEnvironment.Production)
      {
        return "/Resources/Icons/tray.ico";
      }
      return "/Resources/Icons/tray-dev.ico";
    }
  }

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
