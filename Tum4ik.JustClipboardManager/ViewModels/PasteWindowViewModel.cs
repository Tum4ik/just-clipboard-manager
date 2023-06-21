using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class PasteWindowViewModel : TranslationViewModel
{
  private readonly IEventAggregator _eventAggregator;
  private readonly IClipRepository _clipRepository;
  private readonly IPluginsService _pluginsService;

  public PasteWindowViewModel(IEventAggregator eventAggregator,
                              IClipRepository clipRepository,
                              ITranslationService translationService,
                              IPluginsService pluginsService)
    : base(translationService, eventAggregator)
  {
    _eventAggregator = eventAggregator;
    _clipRepository = clipRepository;
    _pluginsService = pluginsService;
  }


  private const int ClipsLoadInitialSize = 15;
  private const int ClipsLoadBatchSize = 10;
  private int _loadedClipsCount;

  private bool _windowDeactivationTriggeredByDataPasting;

  private readonly Dictionary<int, Clip> _dbClips = new();
  public ObservableCollection<ClipDto> Clips { get; } = new();

  
  private string? _search;
  public string? Search
  {
    get => _search;
    set
    {
      if (SetProperty(ref _search, value))
      {
        SearchStarted?.Invoke();
        _dbClips.Clear();
        Clips.Clear();
        _loadedClipsCount = 0;
        LoadNextClipsBatchAsync().Await(e => throw e);
      }
    }
  }

  public event Action? SearchStarted; 


  public async Task LoadNextClipsBatchAsync()
  {
    _loadedClipsCount += await LoadClipsAsync(skip: _loadedClipsCount, take: ClipsLoadBatchSize, search: _search)
      .ConfigureAwait(false);
  }


  [RelayCommand]
  private async Task WindowVisibilityChangedAsync(Visibility visibility)
  {
    if (visibility == Visibility.Visible)
    {
      _windowDeactivationTriggeredByDataPasting = false;
      _loadedClipsCount = await LoadClipsAsync(take: ClipsLoadInitialSize).ConfigureAwait(false);
    }
    else
    {
      Search = null;
      _dbClips.Clear();
      Clips.Clear();
    }
  }


  [RelayCommand]
  private void WindowDeactivated()
  {
    if (_windowDeactivationTriggeredByDataPasting)
    {
      return;
    }

    _eventAggregator.GetEvent<PasteWindowResultEvent>().Publish(Array.Empty<FormattedDataObject>());
  }


  [RelayCommand]
  private async Task PasteDataAsync(ClipDto? clipDto)
  {
    _windowDeactivationTriggeredByDataPasting = true;
    if (clipDto is null || !_dbClips.TryGetValue(clipDto.Id, out var clip))
    {
      return;
    }

    var dataObjects = clip.FormattedDataObjects;
    _eventAggregator.GetEvent<PasteWindowResultEvent>().Publish(dataObjects);
    var now = DateTime.Now;
    clip.ClippedAt = new(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    await _clipRepository.UpdateAsync(clip).ConfigureAwait(false);
  }


  [RelayCommand]
  private async Task DeleteClipAsync(ClipDto? clipDto)
  {
    if (clipDto is not null && _dbClips.TryGetValue(clipDto.Id, out var clip))
    {
      _dbClips.Remove(clipDto.Id);
      Clips.Remove(clipDto);
      await _clipRepository.DeleteAsync(clip).ConfigureAwait(false);
    }
  }


  private async Task<int> LoadClipsAsync(int skip = 0, int take = int.MaxValue, string? search = null)
  {
    var clips = _clipRepository.GetAsync(skip, take, search);
    var loadedCount = 0;
    await foreach (var clip in clips)
    {
      var plugin = _pluginsService.GetPlugin(clip.PluginId);
      if (plugin is null)
      {
        continue;
      }

      _dbClips[clip.Id] = clip;
      Clips.Add(new()
      {
        Id = clip.Id,
        PluginId = clip.PluginId,
        RepresentationData = plugin.RestoreRepresentationData(clip.RepresentationData),
        SearchLabel = clip.SearchLabel
      });
      loadedCount++;
    }
    return loadedCount;
  }
}
