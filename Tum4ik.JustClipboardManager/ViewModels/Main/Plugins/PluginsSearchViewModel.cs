using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;
internal partial class PluginsSearchViewModel : TranslationViewModel, INavigationAware
{
  private readonly IPluginsService _pluginsService;

  public PluginsSearchViewModel(ITranslationService translationService,
                                IEventAggregator eventAggregator,
                                IPluginsService pluginsService)
    : base(translationService, eventAggregator)
  {
    _pluginsService = pluginsService;
  }


  public async void OnNavigatedTo(NavigationContext navigationContext)
  {
    Plugins.Clear();

    try
    {
      await foreach (var pluginDto in _pluginsService.SearchPluginsAsync())
      {
        Plugins.Add(pluginDto);
      }
    }
    catch (HttpRequestException)
    {
      // todo: show message it is impossible to execute search plugins request
    }
    catch (JsonException)
    {
      // todo: show message about wrong JSON file
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
  [ObservableProperty] private string? _installingPluginId;

  private CancellationTokenSource? _installPluginCancellationTokenSource;


  [RelayCommand]
  private async Task InstallPluginAsync(SearchPluginInfoDto plugin)
  {
    _installPluginCancellationTokenSource = new();
    InstallingPluginId = plugin.Id;
    var progress = new Progress<int>(p => PluginInstallationProgress = p);
    try
    {
      await _pluginsService.InstallPluginAsync(
        plugin.DownloadLink, plugin.Id, progress, _installPluginCancellationTokenSource.Token
      ).ConfigureAwait(true);
      plugin.IsInstalled = true;
    }
    catch (TaskCanceledException)
    {
    }
    finally
    {
      _installPluginCancellationTokenSource.Dispose();
      _installPluginCancellationTokenSource = null;
      PluginInstallationProgress = 0;
      InstallingPluginId = null;
    }
  }


  [RelayCommand]
  private void CancelInstallPlugin()
  {
    _installPluginCancellationTokenSource?.Cancel();
  }
}
