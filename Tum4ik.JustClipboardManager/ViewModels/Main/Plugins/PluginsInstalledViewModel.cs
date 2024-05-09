using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;
internal partial class PluginsInstalledViewModel : TranslationViewModel, INavigationAware
{
  private readonly IPluginsService _pluginsService;
  private readonly ILoadableDirectoryModuleCatalog _moduleCatalog;

  public PluginsInstalledViewModel(ITranslationService translationService,
                                   IEventAggregator eventAggregator,
                                   IPluginsService pluginsService,
                                   ILoadableDirectoryModuleCatalog moduleCatalog)
    : base(translationService, eventAggregator)
  {
    _pluginsService = pluginsService;
    _moduleCatalog = moduleCatalog;
  }


  public bool IsNavigationTarget(NavigationContext navigationContext) => true;

  public void OnNavigatedTo(NavigationContext navigationContext)
  {
    LoadPlugins();
  }


  private void LoadPlugins()
  {
    Plugins.Clear();
    foreach (var installedPluginDto in CreatePluginsDto(_pluginsService.InstalledPlugins))
    {
      Plugins.Add(installedPluginDto);
    }
  }


  public void OnNavigatedFrom(NavigationContext navigationContext)
  {
  }


  public ObservableCollection<InstalledPluginInfoDto> Plugins { get; } = [];


  [RelayCommand]
  private void EnableDisablePlugin(InstalledPluginInfoDto plugin)
  {
    if (plugin.IsEnabled)
    {
      plugin.IsEnabled = false;
      _pluginsService.DisablePlugin(plugin.Id);
    }
    else
    {
      plugin.IsEnabled = true;
      _pluginsService.EnablePlugin(plugin.Id);
    }
  }


  [RelayCommand]
  private async Task UninstallPluginAsync(InstalledPluginInfoDto plugin)
  {
    await _pluginsService.UninstallPluginAsync(plugin.Id).ConfigureAwait(true);
    LoadPlugins();
  }


  private IEnumerable<InstalledPluginInfoDto> CreatePluginsDto(IEnumerable<IPlugin> plugins)
  {
    foreach (var plugin in plugins)
    {
      var dto = PluginToDto(plugin);
      if (dto is not null)
      {
        yield return dto;
      }
    }
  }


  private InstalledPluginInfoDto? PluginToDto(IPlugin plugin)
  {
    var pluginInfo = _moduleCatalog.GetPluginInfo(plugin.Id);
    if (pluginInfo is null)
    {
      return null;
    }

    return new InstalledPluginInfoDto
    {
      Id = plugin.Id,
      Name = pluginInfo.Name,
      Version = pluginInfo.Version,
      Author = pluginInfo.Author,
      Description = pluginInfo.Description,
      IsEnabled = _pluginsService.IsPluginEnabled(plugin.Id)
    };
  }
}
