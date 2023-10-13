using System.IO;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IPluginsService : IPluginsRegistryService
{
  IReadOnlyCollection<IPlugin> InstalledPlugins { get; }
  IAsyncEnumerable<SearchPluginInfoDto> SearchPluginsAsync();
  Task UninstallPluginAsync(string id);
  IPlugin? GetPlugin(string id);
  void EnablePlugin(string id);
  void DisablePlugin(string id);
  bool IsPluginInstalled(string id);
  bool IsPluginEnabled(string id);

  Task InstallPluginAsync(Uri downloadLink,
                          string pluginId,
                          IProgress<int>? progress = null,
                          CancellationToken cancellationToken = default);
  Task InstallPluginAsync(FileInfo zipFile,
                          string pluginId,
                          IProgress<int>? progress = null,
                          CancellationToken cancellationToken = default);
}
