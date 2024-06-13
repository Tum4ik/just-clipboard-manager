using System.IO;
using System.IO.Compression;
using System.Reflection;
using Tum4ik.JustClipboardManager.PluginDevKit;

namespace Tum4ik.JustClipboardManager.Services.Plugins;
internal interface IPluginCatalog
{
  IReadOnlyDictionary<Guid, IPlugin> Plugins { get; }
  event EventHandler? PluginsCollectionChanged;
  Task InitializeAsync();
  Task<PluginInstallationResult> LoadPluginAsync(ZipArchive zipArchive,
                                                 Guid pluginId,
                                                 Version pluginVersion,
                                                 IProgress<int>? progress = null,
                                                 CancellationToken cancellationToken = default);

  Task<PluginInstallationResult> LoadPluginModuleAsync(DirectoryInfo pluginDirectory,
                                                       List<Assembly>? alreadyLoadedAssemblies = null);
}
