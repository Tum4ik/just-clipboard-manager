using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;
internal partial class PluginsSearchViewModel : TranslationViewModel, INavigationAware
{
  private readonly IPluginsService _pluginsService;
  private readonly IInfoBarService _infoBarService;
  private readonly IHub _sentryHub;

  public PluginsSearchViewModel(ITranslationService translationService,
                                IEventAggregator eventAggregator,
                                IPluginsService pluginsService,
                                IInfoBarService infoBarService,
                                IHub sentryHub)
    : base(translationService, eventAggregator)
  {
    _pluginsService = pluginsService;
    _infoBarService = infoBarService;
    _sentryHub = sentryHub;
  }


  public async void OnNavigatedTo(NavigationContext navigationContext)
  {
    await LoadPluginsAsync().ConfigureAwait(false);
  }


  private async Task LoadPluginsAsync()
  {
    Plugins.Clear();

    try
    {
      await foreach (var pluginDto in _pluginsService.SearchPluginsAsync().ConfigureAwait(true))
      {
        Plugins.Add(pluginDto);
      }
    }
    catch (HttpRequestException)
    {
      _infoBarService.ShowWarning(
        "ServerConnectionProblem_Body",
        InfoBarActionType.Button,
        "Retry",
        "ServerConnectionProblem_Title",
        r =>
        {
          if (r == InfoBarResult.Action)
          {
            _ = LoadPluginsAsync();
          }
        }
      );
    }
    catch (JsonException e)
    {
      _sentryHub.CaptureException(e, scope => scope.AddBreadcrumb(
        message: "JSON parse exception when loading available plugins from the server",
        type: "info"
      ));
      _infoBarService.ShowCritical("AvailablePluginsInfoLoadProblem_Body", "AvailablePluginsInfoLoadProblem_Title");
    }
    catch (Octokit.NotFoundException e)
    {
      _sentryHub.CaptureException(e);
      _infoBarService.ShowCritical("PluginsListFileMissing_Body");
    }
  }


  public bool IsNavigationTarget(NavigationContext navigationContext)
  {
    return true;
  }

  public void OnNavigatedFrom(NavigationContext navigationContext)
  {
  }


  public ObservableCollection<SearchPluginInfoDto> Plugins { get; } = new();

  [ObservableProperty] private int _pluginInstallationProgress;
  [ObservableProperty] private Guid _installingPluginId;

  private CancellationTokenSource? _installPluginCancellationTokenSource;


  [RelayCommand]
  private async Task InstallPluginAsync(SearchPluginInfoDto plugin)
  {
    _installPluginCancellationTokenSource = new();
    InstallingPluginId = plugin.Id;
    var progress = new Progress<int>(p => PluginInstallationProgress = p);
    var (success, reason) = await _pluginsService.InstallPluginAsync(
      plugin.DownloadLink, plugin.Id, progress, _installPluginCancellationTokenSource.Token
    ).ConfigureAwait(true);

    if (success)
    {
      plugin.IsInstalled = true;
    }
    else
    {
      var (body, title, severity) = reason switch
      {
        PluginInstallationFailReason.Incompatibility
          => ("PluginIncompatibility_Body", "PluginIncompatibility_Title", InfoBarSeverity.Warning),
        PluginInstallationFailReason.CancelledByUser
          => ("PluginInstallationCancelled", null, InfoBarSeverity.Informational),
        PluginInstallationFailReason.InternetConnectionProblem
          => ("ServerConnectionProblem_Body", "PluginDownloadProblem_Title", InfoBarSeverity.Warning),
        PluginInstallationFailReason.SecurityViolation
          => ("PluginSecurityViolation_Body", "PluginSecurityViolation_Title", InfoBarSeverity.Critical),
        PluginInstallationFailReason.EmptyPluginArchive
          => ("EmptyPluginArchive_Body", null, InfoBarSeverity.Warning),
        PluginInstallationFailReason.PluginLoadProblem
          => ("ImpossibleToLoadPlugin_Body", null, InfoBarSeverity.Warning),
        _ => ("PluginInstallationProblem_Body", "PluginInstallationProblem_Title", InfoBarSeverity.Critical),
      };
      _infoBarService.Show(body, title, severity);
    }

    _installPluginCancellationTokenSource.Dispose();
    _installPluginCancellationTokenSource = null;
    PluginInstallationProgress = 0;
    InstallingPluginId = Guid.Empty;
  }


  [RelayCommand]
  private void CancelInstallPlugin()
  {
    _installPluginCancellationTokenSource?.Cancel();
  }
}
