using System.IO;
using System.Reflection;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Prism.Modularity;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;

namespace Tum4ik.JustClipboardManager.Services;

internal class LoadableDirectoryModuleCatalog : DirectoryModuleCatalog, ILoadableDirectoryModuleCatalog
{
  private readonly Harmony _harmony;

  public LoadableDirectoryModuleCatalog(Harmony harmony)
  {
    _harmony = harmony;
    OverrideModuleInfoCreation();
  }


  private static readonly Type _sourceClass = AccessTools.TypeByName("Prism.Modularity.DirectoryModuleCatalog+InnerModuleInfoLoader");
  private static MethodInfo? _original_CreateModuleInfo;
  private static MethodInfo Original_CreateModuleInfo =>
    _original_CreateModuleInfo ??= AccessTools.Method(_sourceClass, "CreateModuleInfo");

  private static readonly Dictionary<Guid, PluginInfo> _pluginIdToPluginInfo = [];


  public PluginInfo? GetPluginInfo(Guid id)
  {
    if (_pluginIdToPluginInfo.TryGetValue(id, out var pluginInfo))
    {
      return pluginInfo;
    }
    return null;
  }


  private void OverrideModuleInfoCreation()
  {
    var original_GetNotAlreadyLoadedModuleInfos = AccessTools.Method(_sourceClass, "GetNotAlreadyLoadedModuleInfos");
    _harmony.Patch(Original_CreateModuleInfo, postfix: new HarmonyMethod(Postfix_CreateModuleInfo));
    _harmony.Patch(original_GetNotAlreadyLoadedModuleInfos, prefix: new HarmonyMethod(Prefix_GetNotAlreadyLoadedModuleInfos));
  }


  private static ModuleInfo Postfix_CreateModuleInfo(ModuleInfo moduleInfo, Type type)
  {
    var pluginAttribute = type.GetCustomAttribute<PluginAttribute>();
    if (pluginAttribute is null
      || !Guid.TryParse(pluginAttribute.Id, out var pluginId)
      || !Version.TryParse(pluginAttribute.Version, out var pluginVersion))
    {
      return null!;
    }

    var pluginName = pluginAttribute.Name;
    var pluginAuthor = pluginAttribute.Author;
    var pluginDescription = pluginAttribute.Description;

    _pluginIdToPluginInfo[pluginId] = new(pluginName, pluginVersion, pluginAuthor, pluginDescription);

    moduleInfo.ModuleName = pluginId.ToString();
    if (pluginId == Guid.Parse("D930D2CD-3FD9-4012-A363-120676E22AFA"))
    {
      // to load "when available"
      return moduleInfo;
    }

    // to load "on demand"
    moduleInfo.InitializationMode = InitializationMode.OnDemand;
    return moduleInfo;
  }


  private static bool Prefix_GetNotAlreadyLoadedModuleInfos(ref IEnumerable<ModuleInfo?> __result,
                                                            DirectoryInfo directory, Type IModuleType)
  {
    var validAssemblies = new List<Assembly>();
    var alreadyLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).ToList();

    var fileInfos = directory
      .GetFiles("*.dll")
      .Where(
        file => !alreadyLoadedAssemblies.Exists(
          assembly => string.Equals(Path.GetFileName(assembly.Location), file.Name, StringComparison.OrdinalIgnoreCase)
        )
      )
      .ToList();

    foreach (FileInfo fileInfo in fileInfos)
    {
      try
      {
        validAssemblies.Add(Assembly.LoadFrom(fileInfo.FullName));
      }
      catch (BadImageFormatException)
      {
        // skip non-.NET Dlls
      }
    }

    var config = ContainerLocator.Container.Resolve<IConfiguration>();
    var devKitAssemblyName = config["Plugins:DevKitAssemblyName"];
    var devKitMinSupportedVersion = config.GetRequiredSection("Plugins:DevKitMinSupportedVersion").Get<Version>();
    var builtInPluginsAssemblyNames = config.GetSection("Plugins:BuiltInPluginsAssemblyNames").Get<string[]>() ?? [];

    __result = validAssemblies
      .Where(assembly =>
      {
        if (builtInPluginsAssemblyNames.Contains(assembly.GetName().Name))
        {
          return true;
        }
        var devKitAssembly = assembly
          .GetReferencedAssemblies()
          .Where(a => a.Name == devKitAssemblyName)
          .FirstOrDefault(a =>
          {
            return a.Version >= devKitMinSupportedVersion;
          });
        return devKitAssembly is not null;
      })
      .SelectMany(assembly =>
      {
        try
        {
          return assembly
            .GetExportedTypes()
            .Where(IModuleType.IsAssignableFrom)
            .Where(t => t != IModuleType)
            .Where(t => !t.IsAbstract)
            .Select(type => (ModuleInfo?) Original_CreateModuleInfo.Invoke(null, [type]))
            .Where(m => m is not null);
        }
        catch (TypeLoadException)
        {
          return [];
        }
      });

    return false;
  }
}


internal record PluginInfo(string Name, Version Version, string? Author, string? Description);
