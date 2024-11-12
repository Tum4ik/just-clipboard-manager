using System.Collections.Frozen;
using Tum4ik.JustClipboardManager.Data.Dto;
using Tum4ik.JustClipboardManager.PluginDevKit;

namespace Tum4ik.JustClipboardManager.Services.Plugins;
internal interface IPluginsService
{
  FrozenDictionary<Guid, IPlugin> EnabledPlugins { get; }
  IPlugin? this[Guid id] { get; }
  FrozenSet<string> EnabledPluginFormats { get; }
  Task InitializeAsync();
  IAsyncEnumerable<SearchPluginInfoDto> SearchPluginsAsync();
  Task UninstallPluginAsync(Guid id);
  Task RestorePluginAsync(Guid id);
  Task EnablePluginAsync(Guid id);
  Task DisablePluginAsync(Guid id);

  Task<PluginInstallationResult> InstallPluginAsync(Uri downloadLink,
                                                    Guid pluginId,
                                                    IProgress<int>? progress = null,
                                                    CancellationToken cancellationToken = default);
}
