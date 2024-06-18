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

  public PluginsService(IGitHubClient gitHubClient,
                        IHttpClientFactory httpClientFactory,
                        IFile file,
                        IPluginRepository pluginRepository,
                        IHub sentryHub,
                        IConfiguration configuration,
                        IPluginCatalog pluginCatalog)
  {
    _gitHubClient = gitHubClient;
    _httpClientFactory = httpClientFactory;
    _file = file;
    _pluginRepository = pluginRepository;
    _sentryHub = sentryHub;
    _configuration = configuration;
    _pluginCatalog = pluginCatalog;

    LoadAvailablePlugins();
    pluginCatalog.PluginsCollectionChanged += (s, e) => LoadAvailablePlugins();
  }


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


  public async Task PreInstallPluginsAsync()
  {
    await InstallDefaultTextPluginAsync().ConfigureAwait(false);

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
    await _pluginRepository.UpdateIsInstalledAsync(id, false).ConfigureAwait(false);
    await LoadAvailablePluginsAsync().ConfigureAwait(false);
  }


  public async Task EnablePluginAsync(Guid id)
  {
    await _pluginRepository.UpdateIsEnabledAsync(id, true).ConfigureAwait(false);
    await LoadAvailablePluginsAsync().ConfigureAwait(false);
  }


  public async Task DisablePluginAsync(Guid id)
  {
    await _pluginRepository.UpdateIsEnabledAsync(id, false).ConfigureAwait(false);
    await LoadAvailablePluginsAsync().ConfigureAwait(false);
  }


  public async Task<PluginInstallationResult> InstallPluginAsync(Uri downloadLink,
                                                                 Guid pluginId,
                                                                 Version pluginVersion,
                                                                 IProgress<int>? progress = null,
                                                                 CancellationToken cancellationToken = default)
  {
    _sentryHub.AddBreadcrumb(
      message: "Installing plugin",
      category: "info",
      type: "info",
      data: new Dictionary<string, string> { { "Plugin id", pluginId.ToString() } }
    );
    if (await _pluginRepository.ExistsAsync(pluginId, pluginVersion).ConfigureAwait(false))
    {
      // todo: improve repositories to avoid those two calls below, must one call
      await _pluginRepository.UpdateIsInstalledAsync(pluginId, true).ConfigureAwait(false);
      await _pluginRepository.UpdateIsEnabledAsync(pluginId, true).ConfigureAwait(false);
      progress?.Report(100);
      return PluginInstallationResult.Success;
    }

    Progress<int>? progress1 = progress is null ? null : new(p => progress.Report(p / 2));
    Progress<int>? progress2 = progress is null ? null : new(p => progress.Report(p / 2 + 50));
    try
    {
      using var memoryStream = await DownloadPluginZipAsync(downloadLink, progress1, cancellationToken).ConfigureAwait(false);
      using var zipArchive = new ZipArchive(memoryStream);

      return await _pluginCatalog
        .LoadPluginAsync(zipArchive, pluginId, pluginVersion, progress2, cancellationToken)
        .ConfigureAwait(false);
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


  private void LoadAvailablePlugins()
  {
    LoadAvailablePluginsAsync().Await(e => _sentryHub.CaptureException(e));
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


  private async Task InstallDefaultTextPluginAsync()
  {
    var textPluginIdStr = _configuration["Plugins:DefaultTextPluginId"]!;
    var textPluginId = Guid.Parse(textPluginIdStr);
    if (await _pluginRepository.ExistsAsync(textPluginId).ConfigureAwait(false))
    {
      return;
    }

    var pluginsDirectory = _configuration["Plugins:FilesDirectory"]!;
    var textPluginDirectory = Path.Combine(pluginsDirectory, textPluginIdStr);
    await _pluginCatalog
      .LoadPluginModuleAsync(new DirectoryInfo(textPluginDirectory))
      .ConfigureAwait(false);
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
