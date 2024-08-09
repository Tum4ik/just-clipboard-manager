using System.IO;
using System.Reflection;
using Tum4ik.JustClipboardManager.PluginDevKit;

namespace Tum4ik.JustClipboardManager.Services.Plugins;
internal interface IPluginCatalog
{
  IReadOnlyDictionary<Guid, IPlugin> Plugins { get; }
  
  Task<(PluginInstallationResult, IPluginModule?)> LoadPluginModuleAsync(DirectoryInfo pluginDirectory,
                                                                         Assembly[]? alreadyLoadedAssemblies = null);
}
