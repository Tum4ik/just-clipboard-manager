using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Tum4ik.EventAggregator;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Mvvm;

namespace Tum4ik.JustClipboardManager.ViewModels;

[INotifyPropertyChanged]
internal partial class PasteWindowViewModel : IWindowAware
{
  private readonly IEventAggregator _eventAggregator;
  private readonly AppDbContext _dbContext;

  public PasteWindowViewModel(IEventAggregator eventAggregator,
                              AppDbContext dbContext)
  {
    _eventAggregator = eventAggregator;
    _dbContext = dbContext;
  }


  private Action? _hideWindow;


  public void WindowActions(Action hide)
  {
    _hideWindow = hide;
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

    _hideWindow?.Invoke();
    _eventAggregator.GetEvent<PasteWindowResultEvent>().Publish(Array.Empty<FormattedDataObject>());
  }


  [RelayCommand]
  private void PasteData(Clip? clip)
  {
    _windowDeactivationTriggeredByDataPasting = true;
    _hideWindow?.Invoke();
    if (clip is null)
    {
      return;
    }

    var dataObjects = clip.FormattedDataObjects;
    _eventAggregator.GetEvent<PasteWindowResultEvent>().Publish(dataObjects);
  }


  private async Task<int> LoadClips(int skip = 0, int take = 0, string? search = null)
  {
    var clips = _dbContext.Clips
      .Where(c =>
        string.IsNullOrEmpty(search)
        || (!string.IsNullOrEmpty(c.SearchLabel) && EF.Functions.Like(c.SearchLabel, $"%{search}%"))
      )
      .OrderByDescending(c => c.ClippedAt)
      .Skip(skip)
      .Take(take)
      .Include(c => c.FormattedDataObjects.OrderBy(fdo => fdo.FormatOrder))
      .AsAsyncEnumerable();
    var loadedCount = 0;
    await foreach (var clip in clips)
    {
      Clips.Add(clip);
      loadedCount++;
    }
    return loadedCount;
  }
}
