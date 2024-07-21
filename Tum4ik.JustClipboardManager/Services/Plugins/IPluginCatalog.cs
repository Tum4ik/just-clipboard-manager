using System.IO;
using System.IO.Compression;
using System.Reflection;
using Tum4ik.JustClipboardManager.PluginDevKit;

namespace Tum4ik.JustClipboardManager.Services.Plugins;
internal interface IPluginCatalog
{
  IReadOnlyDictionary<Guid, IPlugin> Plugins { get; }
  
  Task<PluginInstallationResult> LoadPluginAsync(ZipArchive zipArchive,
                                                 Guid pluginId,
                                                 Version pluginVersion,
                                                 IProgress<int>? progress = null,
                                                 CancellationToken cancellationToken = default);

  Task<PluginInstallationResult> LoadPluginModuleAsync(DirectoryInfo pluginDirectory,
                                                       DirectoryInfo pluginVersionDirectory,
                                                       Assembly[]? alreadyLoadedAssemblies = null,
                                                       bool isBuiltInPlugin = false);
}
