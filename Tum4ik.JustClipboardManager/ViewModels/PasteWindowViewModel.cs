using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tum4ik.EventAggregator;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Events;

namespace Tum4ik.JustClipboardManager.ViewModels;

[INotifyPropertyChanged]
internal partial class PasteWindowViewModel
{
  private readonly IEventAggregator _eventAggregator;
  private readonly IClipRepository _clipRepository;

  public PasteWindowViewModel(IEventAggregator eventAggregator,
                              IClipRepository clipRepository)
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
        _ = LoadNextClipsBatchAsync();
      }
    }
  }

  public event Action? SearchStarted; 


  public async Task LoadNextClipsBatchAsync()
  {
    _loadedClipsCount += await LoadClips(skip: _loadedClipsCount, take: ClipsLoadBatchSize, search: _search)
      .ConfigureAwait(false);
  }


  [RelayCommand]
  private async Task WindowVisibilityChanged(Visibility visibility)
  {
    if (visibility == Visibility.Visible)
    {
      _windowDeactivationTriggeredByDataPasting = false;
      _loadedClipsCount = await LoadClips(take: ClipsLoadInitialSize).ConfigureAwait(false);
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
  private void PasteData(Clip? clip)
  {
    _windowDeactivationTriggeredByDataPasting = true;
    if (clip is null)
    {
      return;
    }

    var dataObjects = clip.FormattedDataObjects;
    _eventAggregator.GetEvent<PasteWindowResultEvent>().Publish(dataObjects);
  }


  [RelayCommand]
  private async Task DeleteClip(Clip? clip)
  {
    if (clip is not null)
    {
      Clips.Remove(clip);
      await _clipRepository.DeleteAsync(clip).ConfigureAwait(false);
    }
  }


  private async Task<int> LoadClips(int skip = 0, int take = int.MaxValue, string? search = null)
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
