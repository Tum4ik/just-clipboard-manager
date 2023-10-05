using System.IO;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.VisualStudio.Threading;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Extensions;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Properties;

namespace Tum4ik.JustClipboardManager.Services;
internal class PluginsService : IPluginsService
{
  private readonly IContainerProvider _containerProvider;
  private readonly IEventAggregator _eventAggregator;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly ILoadableDirectoryModuleCatalog _moduleCatalog;
  private readonly IModuleManager _moduleManager;
  private readonly JoinableTaskFactory _joinableTaskFactory;

  public PluginsService(IContainerProvider containerProvider,
                        IEventAggregator eventAggregator,
                        IHttpClientFactory httpClientFactory,
                        ILoadableDirectoryModuleCatalog moduleCatalog,
                        IModuleManager moduleManager,
                        JoinableTaskFactory joinableTaskFactory)
  {
    _containerProvider = containerProvider;
    _eventAggregator = eventAggregator;
    _httpClientFactory = httpClientFactory;
    _moduleCatalog = moduleCatalog;
    _moduleManager = moduleManager;
    _joinableTaskFactory = joinableTaskFactory;
  }


  private readonly Dictionary<string, IPlugin> _plugins = new();
  private readonly Dictionary<string, bool> _enabledPlugins = new();


  public IReadOnlyCollection<IPlugin> InstalledPlugins => _plugins.Values;


  public void RegisterPlugin(string id)
  {
    _plugins[id] = _containerProvider.ResolvePlugin(id);
    _enabledPlugins[id] = PluginSettings.Default.Get(id, true);
    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  public void UnregisterPlugin(string id)
  {
    _plugins.Remove(id);
    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  public IPlugin? GetPlugin(string id)
  {
    if (_plugins.TryGetValue(id, out var plugin))
    {
      return plugin;
    }
    return null;
  }


  public void EnablePlugin(string id)
  {
    if (_enabledPlugins.TryGetValue(id, out var enabled) && !enabled)
    {
      _enabledPlugins[id] = true;
      PluginSettings.Default.Save(id, true);
    }
  }


  public void DisablePlugin(string id)
  {
    if (_enabledPlugins.TryGetValue(id, out var enabled) && enabled)
    {
      _enabledPlugins[id] = false;
      PluginSettings.Default.Save(id, false);
    }
  }


  public bool IsPluginInstalled(string id)
  {
    return _plugins.ContainsKey(id);
  }


  public bool IsPluginEnabled(string id)
  {
    if (_enabledPlugins.TryGetValue(id, out var enabled))
    {
      return enabled;
    }
    return false;
  }


  public async Task InstallPluginAsync(Uri downloadLink,
                                       string pluginId,
                                       IProgress<int>? progress = null,
                                       CancellationToken cancellationToken = default)
  {
    Progress<int>? progress1 = progress is null ? null : new(p => progress.Report(p / 2));
    Progress<int>? progress2 = progress is null ? null : new(p => progress.Report(p / 2 + 50));
    using var memoryStream = await DownloadPluginZipAsync(downloadLink, progress1, cancellationToken).ConfigureAwait(false);
    await ExtractPluginFilesFromZipAsync(memoryStream, pluginId, progress2, cancellationToken).ConfigureAwait(false);
    
    _moduleCatalog.Load();
    foreach (var module in _moduleCatalog.Modules)
    {
      if (module.State == ModuleState.NotStarted)
      {
        await _joinableTaskFactory.SwitchToMainThreadAsync(cancellationToken: default);
        _moduleManager.LoadModule(module.ModuleName);
        progress?.Report(100);
      }
    }
  }


  public Task InstallPluginAsync(FileInfo zipFile,
                                 string pluginId,
                                 IProgress<int>? progress = null,
                                 CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }


  private async Task<MemoryStream> DownloadPluginZipAsync(Uri downloadLink,
                                                          IProgress<int>? progress = null,
                                                          CancellationToken cancellationToken = default)
  {
    var memoryStream = new MemoryStream();
    using var httpClient = _httpClientFactory.CreateHttpClient();
    using var response = await httpClient.GetAsync(downloadLink, cancellationToken).ConfigureAwait(false);
    var contentLength = response.Content.Headers.ContentLength;
    using var downloadStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    var buffer = new byte[8 * 1024];
    var totalBytesRead = 0L;
    var bytesRead = 0;
    while ((bytesRead = await downloadStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
    {
      await memoryStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
      totalBytesRead += bytesRead;
      if (contentLength is not null)
      {
        progress?.Report((int) ((double) totalBytesRead / contentLength * 100));
      }
    }

    return memoryStream;
  }


  private static async Task ExtractPluginFilesFromZipAsync(Stream zipStream,
                                                           string pluginId,
                                                           IProgress<int>? progress = null,
                                                           CancellationToken cancellationToken = default)
  {
    const string PluginsJsonFileName = "Plugins.json";
    var guid = Guid.NewGuid();
    var pluginFilesStream = new FileStream(PluginsJsonFileName, FileMode.OpenOrCreate);
    Dictionary<string, List<string>> pluginFiles;
    try
    {
      pluginFiles = await JsonSerializer
        .DeserializeAsync<Dictionary<string, List<string>>>(pluginFilesStream, cancellationToken: cancellationToken)
        .ConfigureAwait(false) ?? new();
    }
    catch (JsonException)
    {
      pluginFiles = new Dictionary<string, List<string>>();
    }
    pluginFilesStream.Close();
    var filesList = new List<string>();
    pluginFiles[pluginId] = filesList;

    using var zipArchive = new ZipArchive(zipStream);
    var entriesCount = zipArchive.Entries.Count;
    await Task.Run(() =>
    {
      for (var i = 0; i < entriesCount; i++)
      {
        var entry = zipArchive.Entries[i];
        var entryFullName = entry.FullName;
        if (!File.Exists(entryFullName))
        {
          var directory = Path.GetDirectoryName(entryFullName);
          var fileName = Path.GetFileNameWithoutExtension(entryFullName);
          var extension = Path.GetExtension(entryFullName);
          if (directory is not null && fileName is not null && extension is not null)
          {
            var destinationPath = Path.Combine(directory, $"{guid}_{fileName}{extension}");
            if (destinationPath.StartsWith(directory, StringComparison.Ordinal))
            {
              entry.ExtractToFile(destinationPath, true);
              filesList.Add(destinationPath);
            }
          }
        }

        progress?.Report((int) ((double) i / entriesCount * 100));
      }
    }, cancellationToken).ConfigureAwait(false);

    var serializedPluginFiles = JsonSerializer.Serialize(pluginFiles);
    await File.WriteAllTextAsync(PluginsJsonFileName, serializedPluginFiles, cancellationToken).ConfigureAwait(false);
  }
}
