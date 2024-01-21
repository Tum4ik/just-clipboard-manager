using System.Reflection;
using DryIoc;
using Tum4ik.JustClipboardManager.Exceptions;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;
using Path = System.IO.Path;

namespace Tum4ik.JustClipboardManager.Plugins;
internal sealed class PluginLoader : IPluginLoader
{
  private readonly IRegistrator _registrator;

  public PluginLoader(IRegistrator registrator)
  {
    _registrator = registrator;
  }


  private const string PluginsPath = @".\Plugins";


  public void Load()
  {
    var loaderType = typeof(PluginInfoLoader);
    if (loaderType.Assembly is null)
    {
      throw new PluginLoadingException("Plugin loader type has no assembly.");
    }
    if (loaderType.FullName is null)
    {
      throw new PluginLoadingException("Plugin loader type has no full name.");
    }

    var loader = AppDomain.CurrentDomain
      .CreateInstanceFromAndUnwrap(loaderType.Assembly.Location, loaderType.FullName)
      as PluginInfoLoader;
    if (loader is null)
    {
      throw new PluginLoadingException("Impossible to create the loader instance.");
    }

    var plugins = loader.GetPluginsInfo(new DirectoryInfo(PluginsPath));
    foreach (var plugin in plugins)
    {
      _registrator.Register(typeof(IPlugin), plugin.Type, reuse: Reuse.Singleton, serviceKey: plugin.Id);
    }
  }


  private sealed class PluginInfoLoader : MarshalByRefObject
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
    internal PluginInfo[] GetPluginsInfo(DirectoryInfo directory)
    {
      var iPluginAssembly = AppDomain.CurrentDomain.GetAssemblies()
        .First(a => a.FullName == typeof(IPlugin).Assembly.FullName);
      var iPluginType = iPluginAssembly.GetType(typeof(IPlugin).FullName);

      var alreadyLoadedAssemblies = AppDomain.CurrentDomain
        .GetAssemblies()
        .Where(a => !a.IsDynamic)
        .ToArray();

      Directory.CreateDirectory(directory.FullName);
      var dllFiles = directory.GetFiles("*.dll")
        .Where(f => alreadyLoadedAssemblies.FirstOrDefault(ala => IsEqual(ala, f)) is null);
      var loadedAssemblies = new List<Assembly>();
      foreach (var dllFile in dllFiles)
      {
        try
        {
          loadedAssemblies.Add(Assembly.LoadFrom(dllFile.FullName));
        }
        catch (BadImageFormatException)
        {
          // skip bad assemblies
        }
      }

      return loadedAssemblies.SelectMany(
        a => a.GetExportedTypes()
              .Where(iPluginType.IsAssignableFrom)
              .Where(t => t != iPluginType)
              .Where(t => !t.IsAbstract)
              .Select(t => new PluginInfo { Id = t.GetCustomAttribute<PluginAttribute>().Id, Type = t })
      ).ToArray();
    }


    private static bool IsEqual(Assembly assembly, FileInfo file)
    {
      return string.Equals(Path.GetFileName(assembly.Location), file.Name, StringComparison.OrdinalIgnoreCase);
    }
  }
}
