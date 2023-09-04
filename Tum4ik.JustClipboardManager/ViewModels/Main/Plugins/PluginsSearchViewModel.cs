using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Octokit;
using Prism.Events;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;
internal class PluginsSearchViewModel : TranslationViewModel, INavigationAware
{
  private readonly IGitHubClient _gitHubClient;
  private readonly IPluginsService _pluginsService;

  public PluginsSearchViewModel(ITranslationService translationService,
                                IEventAggregator eventAggregator,
                                IGitHubClient gitHubClient,
                                IPluginsService pluginsService)
    : base(translationService, eventAggregator)
  {
    _gitHubClient = gitHubClient;
    _pluginsService = pluginsService;
  }


  public async void OnNavigatedTo(NavigationContext navigationContext)
  {
    Plugins.Clear();
    var pluginsListJsonBytes = (await _gitHubClient.Repository
      .Content
      .GetRawContent("Tum4ik", "just-clipboard-manager-plugins", "plugins-list.json")
      .ConfigureAwait(true));
    using var stream = new MemoryStream(pluginsListJsonBytes);
    try
    {
      await foreach (var pluginDto in JsonSerializer.DeserializeAsyncEnumerable<SearchPluginInfoDto>(stream))
      {
        if (pluginDto is not null)
        {
          pluginDto.IsInstalled = _pluginsService.IsPluginInstalled(pluginDto.Id);
          Plugins.Add(pluginDto);
        }
      }
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
}
