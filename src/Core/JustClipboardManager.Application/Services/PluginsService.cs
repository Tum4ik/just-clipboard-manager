using System.Reflection;
using System.Runtime.Loader;
using JustClipboardManager.PluginDevKit.Core;

namespace JustClipboardManager.Application.Services;

public abstract class PluginsService<TClipboardContent>
{
  private readonly Dictionary<Guid, LoadedPlugin<TClipboardContent>> _loadedPlugins = [];

  public void LoadPlugins()
  {
    var pluginsDirs = Directory.GetDirectories(
      Path.Combine(
        Directory.GetParent(Assembly.GetEntryAssembly()!.Location)!.FullName,
        "Plugins"
      )
    );
    foreach (var pluginDir in pluginsDirs)
    {
      var dllFiles = Directory.GetFiles(pluginDir, "*.dll");
      var loadContext = new AssemblyLoadContext(null, isCollectible: true);
      var weakReference = new WeakReference(loadContext, trackResurrection: true);
      foreach (var dllFile in dllFiles)
      {
        var assembly = loadContext.LoadFromAssemblyPath(dllFile);
        var pluginType = assembly.GetTypes()
          .FirstOrDefault(
            t => IsAssignableFromOpenGeneric(typeof(JcmPlugin<>), t)
                 && t.IsClass
                 && !t.IsAbstract
          );
        if (pluginType is not null)
        {
          var plugin = (JcmPlugin<TClipboardContent>)Activator.CreateInstance(pluginType)!;
          var loadedPlugin = new LoadedPlugin<TClipboardContent>
          {
            LoadContext = loadContext,
            WeakReference = weakReference,
            Plugin = plugin
          };
          _loadedPlugins.Add(plugin.Id, loadedPlugin);
        }
      }
    }
  }


  private static bool IsAssignableFromOpenGeneric(Type openGenericType, Type? typeToCheck)
  {
    while (typeToCheck != null && typeToCheck != typeof(object))
    {
      // Check if the current type in the chain is a generic type
      if (typeToCheck.IsGenericType)
      {
        // Compare its open generic definition to our target
        if (typeToCheck.GetGenericTypeDefinition() == openGenericType)
        {
          return true;
        }
      }

      // Move up the inheritance hierarchy
      typeToCheck = typeToCheck.BaseType;
    }
    return false;
  }
}

internal class LoadedPlugin<TClipboardContent>
{
  public required AssemblyLoadContext LoadContext { get; init; }
  public required WeakReference WeakReference { get; init; }
  public required JcmPlugin<TClipboardContent> Plugin { get; init; }
}
