using System.IO;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPluginsService : IPluginsRegistryService
{
  IReadOnlyCollection<IPlugin> InstalledPlugins { get; }
  Task PreInstallPluginsAsync();
  IAsyncEnumerable<SearchPluginInfoDto> SearchPluginsAsync();
  Task UninstallPluginAsync(Guid id);
  IPlugin? GetPlugin(Guid id);
  void EnablePlugin(Guid id);
  void DisablePlugin(Guid id);
  bool IsPluginInstalled(Guid id);
  bool IsPluginEnabled(Guid id);

  Task<PluginInstallationResult> InstallPluginAsync(Uri downloadLink,
                                                    Guid pluginId,
                                                    IProgress<int>? progress = null,
                                                    CancellationToken cancellationToken = default);
  Task<PluginInstallationResult> InstallPluginAsync(FileInfo zipFile,
                                                    Guid pluginId,
                                                    IProgress<int>? progress = null,
                                                    CancellationToken cancellationToken = default);
}


internal record PluginInstallationResult(
  bool Success,
  PluginInstallationFailReason FailReason = PluginInstallationFailReason.None
);

internal enum PluginInstallationFailReason
{
  None,

  Incompatibility,
  CancelledByUser,
  InternetConnectionProblem,
  SecurityViolation,
  EmptyPluginArchive,
  PluginLoadProblem,

  OtherProblem
}
