using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Threading;
using Octokit;
using Prism.Events;
using Prism.Modularity;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Exceptions;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.Properties;

namespace Tum4ik.JustClipboardManager.Services;
internal class PluginsService : IPluginsService
{
  internal static readonly Guid DefaultTextPluginId = new("D930D2CD-3FD9-4012-A363-120676E22AFA");

  private readonly IEventAggregator _eventAggregator;
  private readonly IGitHubClient _gitHubClient;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly ILoadableDirectoryModuleCatalog _moduleCatalog;
  private readonly IModuleManager _moduleManager;
  private readonly JoinableTaskFactory _joinableTaskFactory;
  private readonly IFile _file;
  private readonly IPluginRepository _pluginRepository;
  private readonly IHub _sentryHub;
  private readonly IConfiguration _configuration;

  public PluginsService(IEventAggregator eventAggregator,
                        IGitHubClient gitHubClient,
                        IHttpClientFactory httpClientFactory,
                        ILoadableDirectoryModuleCatalog moduleCatalog,
                        IModuleManager moduleManager,
                        JoinableTaskFactory joinableTaskFactory,
                        IFile file,
                        IPluginRepository pluginRepository,
                        IHub sentryHub,
                        IConfiguration configuration)
  {
    _eventAggregator = eventAggregator;
    _gitHubClient = gitHubClient;
    _httpClientFactory = httpClientFactory;
    _moduleCatalog = moduleCatalog;
    _moduleManager = moduleManager;
    _joinableTaskFactory = joinableTaskFactory;
    _file = file;
    _pluginRepository = pluginRepository;
    _sentryHub = sentryHub;
    _configuration = configuration;
  }


  private readonly Dictionary<Guid, IPlugin> _plugins = [];
  private readonly Dictionary<Guid, bool> _enabledPlugins = [];


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


  public async Task PreInstallPluginsAsync()
  {
    var builtInPlugins = _moduleCatalog.Modules.Where(m => m.InitializationMode == InitializationMode.WhenAvailable);
    foreach (var builtInPlugin in builtInPlugins)
    {
      if (Guid.TryParse(builtInPlugin.ModuleName, out var pluginId)
        && !await _pluginRepository.ExistsAsync(pluginId).ConfigureAwait(false))
      {
        var pluginInfo = _moduleCatalog.GetPluginInfo(pluginId);
        if (pluginInfo is not null)
        {
          await _pluginRepository.AddAsync(new()
          {
            Id = pluginId,
            Name = pluginInfo.Name,
            Version = pluginInfo.Version.ToString(),
            IsInstalled = true
          }).ConfigureAwait(false);
        }
      }
    }

    const string FileName = "pre-install-plugins";
    if (!_file.Exists(FileName))
    {
      return;
    }

    var pluginIdsToInstall = (await _file.ReadAllLinesAsync(FileName).ConfigureAwait(false))
      .Select(line => Guid.TryParse(line, out var guid) ? guid : Guid.Empty)
      .Where(guid => guid != Guid.Empty)
      .ToHashSet();
    await foreach (var pluginDto in SearchPluginsAsync().ConfigureAwait(false))
    {
      if (pluginIdsToInstall.Contains(pluginDto.Id))
      {
        await InstallPluginAsync(pluginDto.DownloadLink, pluginDto.Id).ConfigureAwait(false);
      }
    }
    _file.Delete(FileName);
  }


  public async IAsyncEnumerable<SearchPluginInfoDto> SearchPluginsAsync()
  {
    var devKitMinSupportedVersion = _configuration["Plugins:DevKitMinSupportedVersion"];
    var pluginsListFileName = $"plugins-list-{devKitMinSupportedVersion}.json";
    _sentryHub.AddBreadcrumb(
      message: $"Plugins list file name: {pluginsListFileName}",
      type: "info",
      category: "info"
    );
    var pluginsListJsonBytes = await _gitHubClient.Repository
      .Content
      .GetRawContent("Tum4ik", "just-clipboard-manager-plugins", pluginsListFileName)
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


  public void RegisterPlugin(IPlugin plugin)
  {
    var id = plugin.Id;
    _plugins[id] = plugin;
    _enabledPlugins[id] = PluginSettings.Default.Get(id, true);
    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  public async Task UninstallPluginAsync(Guid id)
  {
    _plugins.Remove(id);
    _enabledPlugins.Remove(id);

    await _pluginRepository.UpdateIsInstalledAsync(id, false).ConfigureAwait(false);
    _moduleCatalog.Modules.First(m => m.ModuleName == id.ToString()).State = ModuleState.NotStarted;

    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  // todo: maybe add option to get enabled/disabled plugins instead using IsPluginEnabled method
  public IPlugin? GetPlugin(Guid id)
  {
    if (_plugins.TryGetValue(id, out var plugin))
    {
      return plugin;
    }
    return null;
  }


  public void EnablePlugin(Guid id)
  {
    if (_enabledPlugins.TryGetValue(id, out var enabled) && !enabled)
    {
      _enabledPlugins[id] = true;
      PluginSettings.Default.Save(id, true);
    }
  }


  public void DisablePlugin(Guid id)
  {
    if (_enabledPlugins.TryGetValue(id, out var enabled) && enabled)
    {
      _enabledPlugins[id] = false;
      PluginSettings.Default.Save(id, false);
    }
  }


  public bool IsPluginInstalled(Guid id)
  {
    return _plugins.ContainsKey(id);
  }


  public bool IsPluginEnabled(Guid id)
  {
    if (_enabledPlugins.TryGetValue(id, out var enabled))
    {
      return enabled;
    }
    return false;
  }


  public async Task<PluginInstallationResult> InstallPluginAsync(Uri downloadLink,
                                                                 Guid pluginId,
                                                                 IProgress<int>? progress = null,
                                                                 CancellationToken cancellationToken = default)
  {
    _sentryHub.AddBreadcrumb(
      message: "Installing plugin",
      category: "info",
      type: "info",
      data: new Dictionary<string, string> { { "Plugin id", pluginId.ToString() } }
    );
    Progress<int>? progress1 = progress is null ? null : new(p => progress.Report(p / 2));
    Progress<int>? progress2 = progress is null ? null : new(p => progress.Report(p / 2 + 50));
    List<string>? pluginFiles = null;
    var success = false;
    try
    {
      using var memoryStream = await DownloadPluginZipAsync(downloadLink, progress1, cancellationToken).ConfigureAwait(false);
      pluginFiles = await ExtractPluginFilesFromZipAsync(memoryStream, pluginId, progress2, cancellationToken).ConfigureAwait(false);

      _moduleCatalog.Load();
      await _joinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
      _moduleManager.LoadModule(pluginId.ToString());

      if (pluginFiles.Count <= 0)
      {
        _sentryHub.CaptureMessage("Plugin archive is empty", SentryLevel.Warning);
        return new(false, PluginInstallationFailReason.EmptyPluginArchive);
      }
      var pluginInfo = _moduleCatalog.GetPluginInfo(pluginId);
      if (pluginInfo is null)
      {
        _sentryHub.CaptureMessage("Module catalog does not contain the plugin", SentryLevel.Warning);
        return new(false, PluginInstallationFailReason.PluginLoadProblem);
      }

      if (await _pluginRepository.ExistsAsync(pluginId).ConfigureAwait(false))
      {
        await _pluginRepository.UpdateIsInstalledAsync(pluginId, true).ConfigureAwait(false);
      }
      else
      {
        var plugin = new Data.Models.Plugin
        {
          Id = pluginId,
          Name = pluginInfo.Name,
          Version = pluginInfo.Version.ToString(),
          IsInstalled = true,
          Files = pluginFiles.Select(pf => new Data.Models.PluginFile { RelativePath = pf }).ToList()
        };
        await _pluginRepository.AddAsync(plugin).ConfigureAwait(false);
        EnablePlugin(pluginId);
      }

      success = true;
      return new(true);
    }
    catch (ModuleNotFoundException e)
    {
      _sentryHub.CaptureException(e);
      return new(false, PluginInstallationFailReason.Incompatibility);
    }
    catch (TaskCanceledException)
    {
      return new(false, PluginInstallationFailReason.CancelledByUser);
    }
    catch (HttpRequestException)
    {
      return new(false, PluginInstallationFailReason.InternetConnectionProblem);
    }
    catch (PluginZipSecurityException e)
    {
      _sentryHub.CaptureException(e);
      return new(false, PluginInstallationFailReason.SecurityViolation);
    }
    catch (Exception e)
    {
      _sentryHub.CaptureException(e, scope => scope.AddBreadcrumb(
        message: "Unpredictable error when installing plugin",
        type: "info"
      ));
      return new(false, PluginInstallationFailReason.OtherProblem);
    }
    finally
    {
      if (!success && pluginFiles is not null)
      {
        foreach (var pluginFile in pluginFiles)
        {
          _file.Delete(pluginFile);
        }
      }
      progress?.Report(100);
    }
  }


  public Task<PluginInstallationResult> InstallPluginAsync(FileInfo zipFile,
                                                           Guid pluginId,
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


  private static async Task<List<string>> ExtractPluginFilesFromZipAsync(
    Stream zipStream,
    Guid pluginId,
    IProgress<int>? progress = null,
    CancellationToken cancellationToken = default
  )
  {
    var filesList = new List<string>();

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
              }
            }
          }

          progress?.Report((int) ((double) i / entriesCount * 100));
        }
      }, cancellationToken).ConfigureAwait(false);
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

    return filesList;
  }
}
