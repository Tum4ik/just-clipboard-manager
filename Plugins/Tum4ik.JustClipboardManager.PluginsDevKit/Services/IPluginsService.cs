using System.IO;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;
public interface IPluginsService
{
  IReadOnlyCollection<IPlugin> InstalledPlugins { get; }
  void RegisterPlugin(string id);
  void UnregisterPlugin(string id);
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
