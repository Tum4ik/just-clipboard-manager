using System.Collections.Frozen;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Threading;
using Octokit;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.PluginDevKit;

namespace Tum4ik.JustClipboardManager.Services.Plugins;
internal class PluginsService : IPluginsService
{
  private readonly IGitHubClient _gitHubClient;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly IFile _file;
  private readonly IPluginRepository _pluginRepository;
  private readonly IHub _sentryHub;
  private readonly IConfiguration _configuration;
  private readonly IPluginCatalog _pluginCatalog;
  private readonly IAppDomain _appDomain;

  public PluginsService(IGitHubClient gitHubClient,
                        IHttpClientFactory httpClientFactory,
                        IFile file,
                        IPluginRepository pluginRepository,
                        IHub sentryHub,
                        IConfiguration configuration,
                        IPluginCatalog pluginCatalog,
                        IAppDomain appDomain)
  {
    _gitHubClient = gitHubClient;
    _httpClientFactory = httpClientFactory;
    _file = file;
    _pluginRepository = pluginRepository;
    _sentryHub = sentryHub;
    _configuration = configuration;
    _pluginCatalog = pluginCatalog;
    _appDomain = appDomain;

    _pluginsDirectoryName = configuration["Plugins:FilesDirectory"]!;
  }


  private bool _isInitialized;
  private readonly string _pluginsDirectoryName;


  public FrozenDictionary<Guid, IPlugin> EnabledPlugins { get; private set; } = null!;
  public IPlugin? this[Guid id]
  {
    get
    {
      if (EnabledPlugins.TryGetValue(id, out var plugin))
      {
        return plugin;
      }
      return null;
    }
  }
  public FrozenSet<string> EnabledPluginFormats { get; private set; } = null!;


  public async Task InitializeAsync()
  {
    if (_isInitialized)
    {
      return;
    }

    await RemoveUninstalledPluginsFilesAsync().ConfigureAwait(false);
    await PreInstallAndUpdatePluginsAsync().ConfigureAwait(false);

    var alreadyLoadedAssemblies = _appDomain.GetLoadedAssemblies();
    await foreach (var installedPlugin in _pluginRepository.GetInstalledPluginsAsync().ConfigureAwait(false))
    {
      var pluginDirectory = new DirectoryInfo(installedPlugin.FilesDirectory);
      var (result, _) = await _pluginCatalog.LoadPluginModuleAsync(pluginDirectory, alreadyLoadedAssemblies).ConfigureAwait(false);
      if (result != PluginInstallationResult.Success)
      {
        await _pluginRepository.UpdateAsync(installedPlugin.Id, x => x.SetProperty(p => p.IsInstalled, false)).ConfigureAwait(false);
      }
    }

    await LoadAvailablePluginsAsync().ConfigureAwait(false);
    _isInitialized = true;
  }


  private async Task PreInstallAndUpdatePluginsAsync()
  {
    await InstallDefaultTextPluginAsync().ConfigureAwait(false);

    const string FileName = "pre-install-plugins";
    HashSet<Guid>? pluginIdsToInstall = null;
    if (_file.Exists(FileName))
    {
      pluginIdsToInstall = (await _file.ReadAllLinesAsync(FileName).ConfigureAwait(false))
        .Select(line => Guid.TryParse(line, out var guid) ? guid : Guid.Empty)
        .Where(guid => guid != Guid.Empty)
        .ToHashSet();
    }
    var installedPlugins = _pluginRepository.GetInstalledPluginsAsync()
      .ToBlockingEnumerable()
      .ToDictionary(p => p.Id);

    await foreach (var pluginDto in SearchPluginsAsync().ConfigureAwait(false))
    {
      var shouldInstallPlugin = pluginIdsToInstall is not null && pluginIdsToInstall.Contains(pluginDto.Id);
      var shouldUpdatePlugin = installedPlugins.TryGetValue(pluginDto.Id, out var installedPlugin)
        && Version.TryParse(installedPlugin.Version, out var installedPluginVersion)
        && pluginDto.Version > installedPluginVersion;
      if (shouldInstallPlugin || shouldUpdatePlugin)
      {
        await InstallPluginAsync(pluginDto.DownloadLink, pluginDto.Id, pluginDto.Version).ConfigureAwait(false);
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
        pluginDto.IsInstalled = await _pluginRepository.IsInstalledAsync(pluginDto.Id).ConfigureAwait(false);
        yield return pluginDto;
      }
    }
  }


  public async Task UninstallPluginAsync(Guid id)
  {
    await _pluginRepository.UpdateAsync(id, u => u.SetProperty(p => p.IsInstalled, false)).ConfigureAwait(false);
    await LoadAvailablePluginsAsync().ConfigureAwait(false);
  }


  public async Task EnablePluginAsync(Guid id)
  {
    await _pluginRepository.UpdateAsync(id, u => u.SetProperty(p => p.IsEnabled, true)).ConfigureAwait(false);
    await LoadAvailablePluginsAsync().ConfigureAwait(false);
  }


  public async Task DisablePluginAsync(Guid id)
  {
    await _pluginRepository.UpdateAsync(id, u => u.SetProperty(p => p.IsEnabled, false)).ConfigureAwait(false);
    await LoadAvailablePluginsAsync().ConfigureAwait(false);
  }


  public async Task<PluginInstallationResult> InstallPluginAsync(
    Uri downloadLink,
    Guid pluginId,
    Version pluginVersion,
    IProgress<int>? progress = null,
    CancellationToken cancellationToken = default
  )
  {
    _sentryHub.AddBreadcrumb(
      message: "Installing plugin",
      category: "info",
      type: "info",
      data: new Dictionary<string, string> { { "Plugin id", pluginId.ToString() } }
    );

    Progress<int>? progress1 = progress is null ? null : new(p => progress.Report(p / 2));
    Progress<int>? progress2 = progress is null ? null : new(p => progress.Report(p / 2 + 50));
    try
    {
      using var memoryStream = await DownloadPluginZipAsync(downloadLink, progress1, cancellationToken).ConfigureAwait(false);
      using var zipArchive = new ZipArchive(memoryStream);

      var destinationPluginDirectoryInfo = new DirectoryInfo(
        Path.Combine(_pluginsDirectoryName, pluginId.ToString().ToUpperInvariant())
      );
      var result = await InstallFromZipAsync(
        zipArchive, destinationPluginDirectoryInfo, progress2, cancellationToken
      ).ConfigureAwait(false);

      if (result != PluginInstallationResult.Success)
      {
        return result;
      }

      IPluginModule? pluginModule;
      (result, pluginModule) = await _pluginCatalog.LoadPluginModuleAsync(destinationPluginDirectoryInfo).ConfigureAwait(false);
      if (result == PluginInstallationResult.Success
        && pluginModule is not null
        && !await _pluginRepository.ExistsAsync(pluginId).ConfigureAwait(false))
      {
        await _pluginRepository.AddAsync(new()
        {
          Id = pluginId,
          Name = pluginModule.Name,
          Version = pluginModule.Version.ToString(),
          Author = pluginModule.Author,
          Description = pluginModule.Description,
          FilesDirectory = destinationPluginDirectoryInfo.FullName
        }).ConfigureAwait(false);
      }

      if (result == PluginInstallationResult.Success && _isInitialized)
      {
        await LoadAvailablePluginsAsync().ConfigureAwait(false);
      }

      return result;
    }
    catch (OperationCanceledException)
    {
      return PluginInstallationResult.CancelledByUser;
    }
    catch (HttpRequestException)
    {
      return PluginInstallationResult.InternetConnectionProblem;
    }
    catch (Exception e)
    {
      _sentryHub.CaptureException(e, scope => scope.AddBreadcrumb(
        message: "Unpredictable error when installing plugin",
        type: "info"
      ));
      return PluginInstallationResult.OtherProblem;
    }
    finally
    {
      progress?.Report(100);
    }
  }


  private static async Task<PluginInstallationResult> InstallFromZipAsync(ZipArchive zipArchive,
                                                                          DirectoryInfo destinationPluginDirectoryInfo,
                                                                          IProgress<int>? progress = null,
                                                                          CancellationToken cancellationToken = default)
  {
    var entriesCount = zipArchive.Entries.Count;
    if (entriesCount <= 0)
    {
      return PluginInstallationResult.EmptyArchive;
    }
    if (entriesCount > 10_000)
    {
      return PluginInstallationResult.ExceededArchiveEntriesCount;
    }

    try
    {
      var totalUncompressedArchiveSize = 0L;
      for (var i = 0; i < entriesCount; i++)
      {
        var entry = zipArchive.Entries[i];
        var entryFullName = entry.FullName;

        var directory = Path.GetDirectoryName(entryFullName);
        var fileName = Path.GetFileName(entryFullName);
        if (directory is not null && fileName is not null)
        {
          var destinationDirectoryPath = Path.GetFullPath(
            Path.Combine(destinationPluginDirectoryInfo.FullName, directory)
          );
          if (!destinationDirectoryPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
          {
            destinationDirectoryPath += Path.DirectorySeparatorChar;
          }
          var destinationFilePath = Path.GetFullPath(Path.Combine(destinationDirectoryPath, fileName));

          if (!destinationFilePath.StartsWith(destinationDirectoryPath, StringComparison.Ordinal))
          {
            return PluginInstallationResult.PotentialConfigChangesAttack;
          }

          Directory.CreateDirectory(destinationDirectoryPath);

          using var fileStream = File.Create(destinationFilePath);
          using var entryStream = entry.Open();
          await entryStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);

          var uncompressedFileSize = new FileInfo(destinationFilePath).Length;
          var compressionRatio = (double) uncompressedFileSize / entry.CompressedLength;
          totalUncompressedArchiveSize += uncompressedFileSize;
          var isCompressionRatioViolation = compressionRatio > 10;
          var isTotalUncompressedArchiveSizeViolation = totalUncompressedArchiveSize > 1024 * 1024 * 1024; // 1 GB
          if (isCompressionRatioViolation || isTotalUncompressedArchiveSizeViolation)
          {
            destinationPluginDirectoryInfo.Delete(true);
            if (isCompressionRatioViolation)
            {
              return PluginInstallationResult.AbnormalArchiveCompressionRatio;
            }
            if (isTotalUncompressedArchiveSizeViolation)
            {
              return PluginInstallationResult.ExceededUncompressedArchiveSize;
            }
          }
        }

        progress?.Report((int) ((double) i / entriesCount * 100));
      }
    }
    catch (OperationCanceledException)
    {
      // delete already extracted files
      destinationPluginDirectoryInfo.Delete(true);
      return PluginInstallationResult.CancelledByUser;
    }

    return PluginInstallationResult.Success;
  }


  private async Task LoadAvailablePluginsAsync()
  {
    var enabledPlugins = new Dictionary<Guid, IPlugin>();
    var enabledPluginFormats = new HashSet<string>();
    foreach (var (pluginId, plugin) in _pluginCatalog.Plugins)
    {
      if (await _pluginRepository.IsInstalledAndEnabledAsync(pluginId).ConfigureAwait(false))
      {
        enabledPlugins[pluginId] = plugin;
        enabledPluginFormats.UnionWith(plugin.Formats);
      }
    }

    EnabledPlugins = enabledPlugins.ToFrozenDictionary();
    EnabledPluginFormats = enabledPluginFormats.ToFrozenSet();
  }


  private async Task RemoveUninstalledPluginsFilesAsync()
  {
    await foreach (var uninstalledPlugin in _pluginRepository.GetUninstalledPluginsAsync().ConfigureAwait(false))
    {
      Directory.Delete(uninstalledPlugin.FilesDirectory, true);
    }
    await _pluginRepository.DeleteUninstalledPluginsAsync().ConfigureAwait(false);
  }


  private async Task InstallDefaultTextPluginAsync()
  {
    var textPluginIdStr = _configuration["Plugins:DefaultTextPluginId"]!;
    var textPluginId = Guid.Parse(textPluginIdStr);
    if (await _pluginRepository.ExistsAsync(textPluginId).ConfigureAwait(false))
    {
      return;
    }

    var pluginsDirectory = _configuration["Plugins:FilesDirectory"]!;
    var textPluginDirectory = new DirectoryInfo(Path.Combine(pluginsDirectory, textPluginIdStr));
    var (result, pluginModule) = await _pluginCatalog
      .LoadPluginModuleAsync(textPluginDirectory)
      .ConfigureAwait(false);
    if (result == PluginInstallationResult.Success && pluginModule is not null)
    {
      await _pluginRepository.AddAsync(new()
      {
        Id = textPluginId,
        Name = pluginModule.Name,
        Version = pluginModule.Version.ToString(),
        Author = pluginModule.Author,
        Description = pluginModule.Description,
        FilesDirectory = textPluginDirectory.FullName
      }).ConfigureAwait(false);
    }
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
}
