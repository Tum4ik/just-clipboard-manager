using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Services.Plugins;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;
internal partial class PluginsInstalledViewModel : TranslationViewModel, INavigationAware
{
  private readonly IPluginRepository _pluginRepository;
  private readonly IPluginsService _pluginsService;

  public PluginsInstalledViewModel(ITranslationService translationService,
                                   IEventAggregator eventAggregator,
                                   IPluginRepository pluginRepository,
                                   IPluginsService pluginsService)
    : base(translationService, eventAggregator)
  {
    _pluginRepository = pluginRepository;
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
    await foreach (var plugin in _pluginRepository.GetAllAsync())
    {
      Plugins.Add(new()
      {
        Id = plugin.Id,
        Name = plugin.Name,
        Version = plugin.Version,
        IsEnabled = plugin.IsEnabled,
        Author = plugin.Author,
        AuthorEmail = plugin.AuthorEmail,
        Description = plugin.Description,
        IsInstalled = plugin.IsInstalled
      });
    }
  }


  public void OnNavigatedFrom(NavigationContext navigationContext)
  {
  }


  public ObservableCollection<InstalledPluginInfoDto> Plugins { get; } = [];


  [RelayCommand]
  private async Task EnableDisablePluginAsync(InstalledPluginInfoDto plugin)
  {
    if (plugin.IsEnabled)
    {
      plugin.IsEnabled = false;
      await _pluginsService.DisablePluginAsync(plugin.Id).ConfigureAwait(false);
    }
    else
    {
      plugin.IsEnabled = true;
      await _pluginsService.EnablePluginAsync(plugin.Id).ConfigureAwait(false);
    }
  }


  [RelayCommand]
  private async Task UninstallPluginAsync(InstalledPluginInfoDto plugin)
  {
    plugin.IsInstalled = false;
    await _pluginsService.UninstallPluginAsync(plugin.Id).ConfigureAwait(true);
  }


  [RelayCommand]
  private async Task RestorePluginAsync(InstalledPluginInfoDto plugin)
  {
    plugin.IsInstalled = true;
    await _pluginsService.RestorePluginAsync(plugin.Id).ConfigureAwait(true);
  }
}
