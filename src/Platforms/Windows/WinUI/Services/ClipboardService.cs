using System;
using JustClipboardManager.Application.Services;
using Windows.ApplicationModel.DataTransfer;

namespace JustClipboardManager.Services;

internal class ClipboardService : IClipboardService
{
  private readonly IPluginsService<DataPackageView, DataPackage> _pluginsService;
  private readonly IClipsService _clipsService;

  public ClipboardService(IPluginsService<DataPackageView, DataPackage> pluginsService,
                          IClipsService clipsService)
  {
    _pluginsService = pluginsService;
    _clipsService = clipsService;
  }

  public void Initialize()
  {
    Clipboard.ContentChanged += OnClipboardContentChanged;
  }

  private async void OnClipboardContentChanged(object? sender, object e)
  {
    try
    {
      var dataPackageView = Clipboard.GetContent();
      var plugins = _pluginsService.GetLoadedPlugins();
      foreach (var plugin in plugins)
      {
        var processedData = await plugin.ProcessDataAsync(dataPackageView);
        await _clipsService.CreateClipAsync(processedData);
      }
    }
    catch (Exception ex)
    {
      // todo: Handle the exception (e.g., log it)
    }
  }
}
