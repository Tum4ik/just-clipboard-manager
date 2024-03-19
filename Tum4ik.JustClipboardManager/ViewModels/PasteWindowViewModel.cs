using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AppCenter.Crashes;
using Prism.Events;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class PasteWindowViewModel : TranslationViewModel
{
  private readonly IClipRepository _clipRepository;
  private readonly IPluginsService _pluginsService;
  private readonly ISettingsService _settingsService;

  public PasteWindowViewModel(IEventAggregator eventAggregator,
                              IClipRepository clipRepository,
                              ITranslationService translationService,
                              IPluginsService pluginsService,
                              ISettingsService settingsService)
    : base(translationService, eventAggregator)
  {
    _clipRepository = clipRepository;
    _pluginsService = pluginsService;
    _settingsService = settingsService;

    WindowWidth = _settingsService.PasteWindowWidth;
    WindowHeight = _settingsService.PasteWindowHeight;
  }


  private const int ClipsLoadInitialSize = 15;
  private const int ClipsLoadBatchSize = 10;
  private int _loadedClipsCount;

  private bool _windowDeactivationTriggeredByDataPasting;

  private TaskCompletionSource<PasteWindowResult?>? _showPasteWindowTcs;

  private readonly Dictionary<int, Clip> _dbClips = [];
  public ObservableCollection<ClipDto> Clips { get; } = [];

  [ObservableProperty] private bool _isSettingsMode;
  partial void OnIsSettingsModeChanged(bool value)
  {
    if (!value)
    {
      _settingsService.PasteWindowWidth = WindowWidth;
      _settingsService.PasteWindowHeight = WindowHeight;
    }
  }

  [ObservableProperty] private int _windowWidth;
  [ObservableProperty] private int _windowHeight;


  private string? _search;
  public string? Search
  {
    get => _search;
    set
    {
      if (SetProperty(ref _search, value))
      {
        _dbClips.Clear();
        Clips.Clear();
        _loadedClipsCount = 0;
        LoadNextClipsBatchAsync().Await(e => throw e);
      }
    }
  }


  public async Task LoadNextClipsBatchAsync()
  {
    _loadedClipsCount += await LoadClipsAsync(skip: _loadedClipsCount, take: ClipsLoadBatchSize, search: _search)
      .ConfigureAwait(false);
  }


  public Task<PasteWindowResult?> WaitForInputAsync()
  {
    _showPasteWindowTcs = new();
    return _showPasteWindowTcs.Task;
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
      IsSettingsMode = false;
    }
  }


  [RelayCommand]
  private void WindowDeactivated()
  {
    if (_windowDeactivationTriggeredByDataPasting)
    {
      return;
    }

    SetInputResult(null);
  }


  [RelayCommand]
  private void Settings()
  {
    IsSettingsMode = !IsSettingsMode;
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
    SetInputResult(new()
    {
      FormattedDataObjects = dataObjects,
      AdditionalInfo = clip.AdditionalInfo
    });
    var now = DateTime.Now;
    clip.ClippedAt = new(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Local);
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


  private void SetInputResult(PasteWindowResult? result)
  {
    _showPasteWindowTcs?.SetResult(result);
  }


  private async Task<int> LoadClipsAsync(int skip = 0, int take = int.MaxValue, string? search = null)
  {
    var clips = _clipRepository.GetAsync(skip, take, search);
    var loadedCount = 0;
    await foreach (var clip in clips)
    {
      var plugin = _pluginsService.GetPlugin(clip.PluginId);
      if (plugin?.Id is null || !_pluginsService.IsPluginEnabled(plugin.Id))
      {
        continue;
      }

      _dbClips[clip.Id] = clip;
      try
      {
        var representationData = plugin.RestoreRepresentationData(clip.RepresentationData, clip.AdditionalInfo);
        Clips.Add(new()
        {
          Id = clip.Id,
          PluginId = clip.PluginId,
          RepresentationData = representationData,
          SearchLabel = clip.SearchLabel
        });
        loadedCount++;
      }
      catch (Exception e)
      {
        Crashes.TrackError(e, new Dictionary<string, string>
        {
          { "Info", "Exception when restore representation data for plugin" },
          { "PluginId", plugin.Id }
        });
      }
    }
    return loadedCount;
  }
}
