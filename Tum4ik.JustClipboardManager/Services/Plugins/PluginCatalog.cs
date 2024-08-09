using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Threading;
using Prism.Ioc;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.PluginDevKit;

namespace Tum4ik.JustClipboardManager.Services.Plugins;
internal class PluginCatalog : IPluginCatalog
{
  private readonly IContainerExtension _containerExtension;
  private readonly IHub _sentryHub;
  private readonly JoinableTaskFactory _joinableTaskFactory;
  private readonly IAppDomain _appDomain;

  public PluginCatalog(IConfiguration configuration,
                       IContainerExtension containerExtension,
                       IHub sentryHub,
                       JoinableTaskFactory joinableTaskFactory,
                       IAppDomain appDomain)
  {
    _containerExtension = containerExtension;
    _sentryHub = sentryHub;
    _joinableTaskFactory = joinableTaskFactory;
    _appDomain = appDomain;
    _devKitAssemblyName = configuration["Plugins:DevKitAssemblyName"]!;
    _devKitMinSupportedVersion = configuration.GetRequiredSection("Plugins:DevKitMinSupportedVersion").Get<Version>()!;
    _defaultTextPluginAssemblyName = configuration["Plugins:DefaultTextPluginAssemblyName"]!;
    Plugins = new ReadOnlyDictionary<Guid, IPlugin>(_plugins);
  }


  private readonly string _devKitAssemblyName;
  private readonly Version _devKitMinSupportedVersion;
  private readonly string _defaultTextPluginAssemblyName;
  private readonly Dictionary<string, IPluginModule> _directoryPathToLoadedModules = [];


  private readonly Dictionary<Guid, IPlugin> _plugins = [];
  public IReadOnlyDictionary<Guid, IPlugin> Plugins { get; }


  public async Task<(PluginInstallationResult, IPluginModule?)>
    LoadPluginModuleAsync(DirectoryInfo pluginDirectory,
                          Assembly[]? alreadyLoadedAssemblies = null)
  {
    if (_directoryPathToLoadedModules.TryGetValue(pluginDirectory.FullName, out var module))
    {
      return (PluginInstallationResult.Success, module);
    }
    IEnumerable<FileInfo> files;
    try
    {
      files = pluginDirectory
        .GetFiles("*.dll", SearchOption.AllDirectories)
        .Where(file => !IsAssemblyFileAlreadyLoaded(file, alreadyLoadedAssemblies ?? _appDomain.GetLoadedAssemblies()));
    }
    catch (DirectoryNotFoundException)
    {
      return (PluginInstallationResult.MissingPluginDirectory, null);
    }

    var loadedAssemblies = new List<Assembly>();
    foreach (FileInfo fileInfo in files)
    {
      try
      {
        loadedAssemblies.Add(Assembly.LoadFrom(fileInfo.FullName));
      }
      catch (BadImageFormatException)
      {
        // skip non-.NET Dlls
      }
    }

    var (result, pluginModule) = await LoadPluginModuleFromLoadedAssembliesAsync(loadedAssemblies).ConfigureAwait(false);
    if (result == PluginInstallationResult.Success && pluginModule is not null)
    {
      _directoryPathToLoadedModules[pluginDirectory.FullName] = pluginModule;
    }

    return (result, pluginModule);
  }


  private async Task<(PluginInstallationResult, IPluginModule?)>
    LoadPluginModuleFromLoadedAssembliesAsync(List<Assembly> loadedAssemblies)
  {
    var pluginAssembly = loadedAssemblies.FirstOrDefault(assembly =>
      assembly.GetName().Name == _defaultTextPluginAssemblyName
      ||
      assembly
        .GetReferencedAssemblies()
        .Any(a => a.Name == _devKitAssemblyName && a.Version >= _devKitMinSupportedVersion)
    );

    if (pluginAssembly is null)
    {
      return (PluginInstallationResult.Incompatibility, null);
    }

    try
    {
      _appDomain.CurrentDomain.AssemblyResolve += MyResolveEventHandler;
      var pluginModuleType = pluginAssembly
        .GetExportedTypes()
        .FirstOrDefault(t => typeof(IPluginModule).IsAssignableFrom(t)
                             && t != typeof(IPluginModule)
                             && !t.IsAbstract);
      if (pluginModuleType is null)
      {
        return (PluginInstallationResult.MissingPluginModuleType, null);
      }

      var pluginModule = (IPluginModule?) Activator.CreateInstance(pluginModuleType);
      if (pluginModule is null)
      {
        return (PluginInstallationResult.PluginModuleInstanceCreationProblem, null);
      }
      var plugin = await ConstructPluginAsync(pluginModule).ConfigureAwait(false);
      var pluginId = pluginModule.Id;
      _plugins[pluginId] = plugin;

      return (PluginInstallationResult.Success, pluginModule);
    }
    catch (TypeLoadException e)
    {
      _sentryHub.CaptureException(e);
      return (PluginInstallationResult.TypesLoadingProblem, null);
    }
    catch (Exception e)
    {
      _sentryHub.CaptureException(e);
      return (PluginInstallationResult.OtherProblem, null);
    }
    finally
    {
      _appDomain.CurrentDomain.AssemblyResolve -= MyResolveEventHandler;
    }
  }


  private static bool IsAssemblyFileAlreadyLoaded(FileInfo file, Assembly[] alreadyLoadedAssemblies)
  {
    return alreadyLoadedAssemblies.Any(
      assembly => string.Equals(Path.GetFileName(assembly.Location), file.Name, StringComparison.OrdinalIgnoreCase)
    );
  }


  private Assembly? MyResolveEventHandler(object? sender, ResolveEventArgs args)
  {
    var requiredAssemblyName = args.Name.Split(',')[0];
    var loadedAssemblies = _appDomain.GetLoadedAssemblies();
    return loadedAssemblies.FirstOrDefault(a => a.GetName().Name == requiredAssemblyName);
  }


  private async Task<IPlugin> ConstructPluginAsync(IPluginModule pluginModule)
  {
    await _joinableTaskFactory.SwitchToMainThreadAsync();
    pluginModule.RegisterTypes(_containerExtension);
    pluginModule.OnInitialized(_containerExtension);
    return _containerExtension.Resolve<IPlugin>(pluginModule.Id.ToString());
  }
}
