using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AppCenter.Crashes;
using Prism.Events;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;
internal partial class PluginsInstalledViewModel : TranslationViewModel, INavigationAware
{
  private readonly IPluginsService _pluginsService;

  public PluginsInstalledViewModel(ITranslationService translationService,
                                   IEventAggregator eventAggregator,
                                   IPluginsService pluginsService)
    : base(translationService, eventAggregator)
  {
    _pluginsService = pluginsService;
  }


  public bool IsNavigationTarget(NavigationContext navigationContext) => true;

  public async void OnNavigatedTo(NavigationContext navigationContext)
  {
    await LoadPluginsAsync().ConfigureAwait(false);
  }


  private async Task LoadPluginsAsync()
  {
    Plugins.Clear();
    await foreach (var installedPluginDto in CreatePluginsDtoAsync(_pluginsService.InstalledPlugins))
    {
      Plugins.Add(installedPluginDto);
    }
  }


  public void OnNavigatedFrom(NavigationContext navigationContext)
  {
  }


  public ObservableCollection<InstalledPluginInfoDto> Plugins { get; } = new();


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
    await LoadPluginsAsync().ConfigureAwait(false);
  }


  private async IAsyncEnumerable<InstalledPluginInfoDto> CreatePluginsDtoAsync(IEnumerable<IPlugin> plugins)
  {
    foreach (var plugin in plugins)
    {
      var dto = await PluginToDtoAsync(plugin).ConfigureAwait(false);
      if (dto is not null)
      {
        yield return dto;
      }
    }
  }


  private async Task<InstalledPluginInfoDto?> PluginToDtoAsync(IPlugin plugin)
  {
    return await Task.Run(() =>
    {
      var pluginAttribute = plugin.GetType().GetCustomAttribute<PluginAttribute>();
      if (pluginAttribute is null)
      {
        return null;
      }

      Version version;
      try
      {
        version = Version.Parse(pluginAttribute.Version);
      }
      catch (Exception e)
      when (e is ArgumentNullException
         || e is ArgumentException
         || e is ArgumentOutOfRangeException
         || e is FormatException
         || e is OverflowException)
      {
        Crashes.TrackError(e, new Dictionary<string, string>
        {
          { "Info", "Plugin version parsing problem" }
        });
        return null;
      }

      return new InstalledPluginInfoDto
      {
        Id = pluginAttribute.Id,
        Name = pluginAttribute.Name,
        Version = version,
        Author = pluginAttribute.Author,
        Description = pluginAttribute.Description,
        IsEnabled = _pluginsService.IsPluginEnabled(pluginAttribute.Id)
      };
    }).ConfigureAwait(false);
  }
}
