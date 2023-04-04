using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels;

internal partial class PasteWindowViewModel : TranslationViewModel
{
  private readonly IEventAggregator _eventAggregator;
  private readonly IClipRepository _clipRepository;

  public PasteWindowViewModel(IEventAggregator eventAggregator,
                              IClipRepository clipRepository,
                              ITranslationService translationService)
    : base(translationService)
  {
    _eventAggregator = eventAggregator;
    _clipRepository = clipRepository;
  }


  private const int ClipsLoadInitialSize = 15;
  private const int ClipsLoadBatchSize = 10;
  private int _loadedClipsCount;

  private bool _windowDeactivationTriggeredByDataPasting;

  public ObservableCollection<Clip> Clips { get; } = new();

  
  private string? _search;
  public string? Search
  {
    get => _search;
    set
    {
      if (SetProperty(ref _search, value))
      {
        SearchStarted?.Invoke();
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
  private async Task PasteDataAsync(Clip? clip)
  {
    _windowDeactivationTriggeredByDataPasting = true;
    if (clip is null)
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
  private async Task DeleteClipAsync(Clip? clip)
  {
    if (clip is not null)
    {
      Clips.Remove(clip);
      await _clipRepository.DeleteAsync(clip).ConfigureAwait(false);
    }
  }


  private async Task<int> LoadClipsAsync(int skip = 0, int take = int.MaxValue, string? search = null)
  {
    var clips = _clipRepository.GetAsync(skip, take, search);
    var loadedCount = 0;
    await foreach (var clip in clips)
    {
      Clips.Add(clip);
      loadedCount++;
    }
    return loadedCount;
  }
}
