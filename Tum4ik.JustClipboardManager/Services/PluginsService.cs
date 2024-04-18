using System.IO;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.VisualStudio.Threading;
using Octokit;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Exceptions;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Extensions;
using Tum4ik.JustClipboardManager.Properties;

namespace Tum4ik.JustClipboardManager.Services;
internal class PluginsService : IPluginsService
{
  internal const string PluginsJsonFileName = "Plugins.json";
  internal const string PluginFilesToRemoveFileName = "plugin-files-to-remove";
  internal const string DefaultTextPluginId = "D930D2CD-3FD9-4012-A363-120676E22AFA";

  private readonly IContainerProvider _containerProvider;
  private readonly IEventAggregator _eventAggregator;
  private readonly Lazy<IGitHubClient> _gitHubClient;
  private readonly Lazy<IHttpClientFactory> _httpClientFactory;
  private readonly Lazy<ILoadableDirectoryModuleCatalog> _moduleCatalog;
  private readonly Lazy<IModuleManager> _moduleManager;
  private readonly Lazy<JoinableTaskFactory> _joinableTaskFactory;

  public PluginsService(IContainerProvider containerProvider,
                        IEventAggregator eventAggregator,
                        Lazy<IGitHubClient> gitHubClient,
                        Lazy<IHttpClientFactory> httpClientFactory,
                        Lazy<ILoadableDirectoryModuleCatalog> moduleCatalog,
                        Lazy<IModuleManager> moduleManager,
                        Lazy<JoinableTaskFactory> joinableTaskFactory)
  {
    _containerProvider = containerProvider;
    _eventAggregator = eventAggregator;
    _gitHubClient = gitHubClient;
    _httpClientFactory = httpClientFactory;
    _moduleCatalog = moduleCatalog;
    _moduleManager = moduleManager;
    _joinableTaskFactory = joinableTaskFactory;
  }


  private readonly Dictionary<string, IPlugin> _plugins = new();
  private readonly Dictionary<string, bool> _enabledPlugins = new();


  public IReadOnlyCollection<IPlugin> InstalledPlugins
  {
    get
    {
      var pluginsToReturn = new List<IPlugin>();
      // Default Text Plugin should be the first
      if (_plugins.TryGetValue(DefaultTextPluginId, out var defaultTextPlugin))
      {
        pluginsToReturn.Add(defaultTextPlugin);
      }
      pluginsToReturn.AddRange(_plugins.Values.Where(p => p.Id != DefaultTextPluginId));
      return pluginsToReturn;
    }
  }


  public async IAsyncEnumerable<SearchPluginInfoDto> SearchPluginsAsync()
  {
    var pluginsListJsonBytes = await _gitHubClient.Value.Repository
      .Content
      .GetRawContent("Tum4ik", "just-clipboard-manager-plugins", "plugins-list.json")
      .ConfigureAwait(false);
    using var stream = new MemoryStream(pluginsListJsonBytes);
    await foreach (var pluginDto in JsonSerializer.DeserializeAsyncEnumerable<SearchPluginInfoDto>(stream).ConfigureAwait(false))
    {
      if (pluginDto is not null)
      {
        pluginDto.IsInstalled = IsPluginInstalled(pluginDto.Id);
        yield return pluginDto;
      }
    }
  }


  public void RegisterPlugin(string id)
  {
    _plugins[id] = _containerProvider.ResolvePlugin(id);
    _enabledPlugins[id] = PluginSettings.Default.Get(id, true);
    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  public async Task UninstallPluginAsync(string id)
  {
    _plugins.Remove(id);
    _enabledPlugins.Remove(id);

    Dictionary<string, List<string>> pluginFiles;
    try
    {
      pluginFiles = await GetPluginsFilesAsync(default).ConfigureAwait(false);
    }
    catch (JsonException)
    {
      return;
    }

    var filesToRemove = pluginFiles[id];
    await File.AppendAllLinesAsync(PluginFilesToRemoveFileName, filesToRemove).ConfigureAwait(false);

    pluginFiles.Remove(id);
    await WritePluginsFilesAsync(pluginFiles, default).ConfigureAwait(false);

    _moduleCatalog.Value.Modules.First(m => m.ModuleName == id).State = ModuleState.NotStarted;

    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  // todo: maybe add option to get enabled/disabled plugins instead using IsPluginEnabled method
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

    _moduleCatalog.Value.Load();
    await _joinableTaskFactory.Value.SwitchToMainThreadAsync(cancellationToken);
    var moduleToLoad = _moduleCatalog.Value.Modules.FirstOrDefault(m => m.ModuleName == pluginId && m.State == ModuleState.NotStarted);
    if (moduleToLoad is not null)
    {
      _moduleManager.Value.LoadModule(moduleToLoad.ModuleName);
      EnablePlugin(pluginId);
    }
    progress?.Report(100);
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
    using var httpClient = _httpClientFactory.Value.CreateHttpClient();
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
    Dictionary<string, List<string>> pluginFiles;
    try
    {
      pluginFiles = await GetPluginsFilesAsync(cancellationToken).ConfigureAwait(false);
    }
    catch (JsonException)
    {
      pluginFiles = new Dictionary<string, List<string>>();
    }
    var filesList = new List<string>();
    pluginFiles[pluginId] = filesList;

    HashSet<string>? pluginsFilesToRemove = null;
    if (File.Exists(PluginFilesToRemoveFileName))
    {
      pluginsFilesToRemove = (await File.ReadAllLinesAsync(PluginFilesToRemoveFileName, cancellationToken).ConfigureAwait(false)).ToHashSet();
    }

    using var zipArchive = new ZipArchive(zipStream);
    var entriesCount = zipArchive.Entries.Count;
    if (entriesCount > 10000)
    {
      throw new PluginZipSecurityException("entriesCount > 10000");
    }
    try
    {
      await Task.Run(() =>
      {
        var totalUncompressedArchiveSize = 0L;
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
              var destinationPath = Path.Combine(directory, $"{pluginId}_{fileName}{extension}");
              if (destinationPath.StartsWith(directory, StringComparison.Ordinal))
              {
                try
                {
                  entry.ExtractToFile(destinationPath, true);
                }
                catch (IOException)
                {
                  // should be ignored in case the plugin is installed immediately after uninstallation
                  // and the plugins files are not removed yet
                }

                filesList.Add(destinationPath);
                var uncompressedFileSize = new FileInfo(destinationPath).Length;
                var compressionRatio = (double) uncompressedFileSize / entry.CompressedLength;
                totalUncompressedArchiveSize += uncompressedFileSize;
                var isCompressionRatioViolation = compressionRatio > 10;
                var isTotalUncompressedArchiveSizeViolation = totalUncompressedArchiveSize > 1024 * 1024 * 1024; // 1 GB
                if (isCompressionRatioViolation || isTotalUncompressedArchiveSizeViolation)
                {
                  foreach (var filePath in filesList)
                  {
                    File.Delete(filePath);
                  }
                  if (isCompressionRatioViolation)
                  {
                    throw new PluginZipSecurityException("compressionRatio > 10");
                  }
                  if (isTotalUncompressedArchiveSizeViolation)
                  {
                    throw new PluginZipSecurityException("totalUncompressedArchiveSize > 1 GB");
                  }
                }

                if (pluginsFilesToRemove is not null && pluginsFilesToRemove.Count > 0)
                {
                  pluginsFilesToRemove.Remove(destinationPath);
                }
              }
            }
          }

          progress?.Report((int) ((double) i / entriesCount * 100));
        }
      }, cancellationToken).ConfigureAwait(false);

      await WritePluginsFilesAsync(pluginFiles, cancellationToken).ConfigureAwait(false);
      if (pluginsFilesToRemove is not null)
      {
        if (pluginsFilesToRemove.Count <= 0)
        {
          File.Delete(PluginFilesToRemoveFileName);
        }
        else
        {
          await File.WriteAllLinesAsync(PluginFilesToRemoveFileName, pluginsFilesToRemove, CancellationToken.None).ConfigureAwait(false);
        }
      }
    }
    catch (TaskCanceledException)
    {
      // delete already extracted files
      foreach (var filePath in filesList)
      {
        File.Delete(filePath);
      }
      throw;
    }
  }


  private static async Task<Dictionary<string, List<string>>> GetPluginsFilesAsync(CancellationToken cancellationToken)
  {
    using var pluginFilesStream = new FileStream(PluginsJsonFileName, System.IO.FileMode.OpenOrCreate);
    return await JsonSerializer
      .DeserializeAsync<Dictionary<string, List<string>>>(pluginFilesStream, cancellationToken: cancellationToken)
      .ConfigureAwait(false) ?? new();
  }


  private static async Task WritePluginsFilesAsync(Dictionary<string, List<string>> pluginsFiles,
                                                   CancellationToken cancellationToken)
  {
    var serializedPluginFiles = JsonSerializer.Serialize(pluginsFiles);
    await File.WriteAllTextAsync(PluginsJsonFileName, serializedPluginFiles, cancellationToken).ConfigureAwait(false);
  }
}
