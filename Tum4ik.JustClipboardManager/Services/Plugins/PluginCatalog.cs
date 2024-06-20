using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Threading;
using Prism.Ioc;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.PluginDevKit;

namespace Tum4ik.JustClipboardManager.Services.Plugins;
internal class PluginCatalog : IPluginCatalog
{
  private readonly IPluginRepository _pluginRepository;
  private readonly IContainerExtension _containerExtension;
  private readonly IHub _sentryHub;
  private readonly JoinableTaskFactory _joinableTaskFactory;
  private readonly IAppDomain _appDomain;

  public PluginCatalog(IConfiguration configuration,
                       IPluginRepository pluginRepository,
                       IContainerExtension containerExtension,
                       IHub sentryHub,
                       JoinableTaskFactory joinableTaskFactory,
                       IAppDomain appDomain)
  {
    _pluginRepository = pluginRepository;
    _containerExtension = containerExtension;
    _sentryHub = sentryHub;
    _joinableTaskFactory = joinableTaskFactory;
    _appDomain = appDomain;
    _pluginsDirectoryName = configuration["Plugins:FilesDirectory"]!;
    _devKitAssemblyName = configuration["Plugins:DevKitAssemblyName"]!;
    _devKitMinSupportedVersion = configuration.GetRequiredSection("Plugins:DevKitMinSupportedVersion").Get<Version>()!;
    _defaultTextPluginAssemblyName = configuration["Plugins:DefaultTextPluginAssemblyName"]!;
    Plugins = new ReadOnlyDictionary<Guid, IPlugin>(_plugins);
  }


  
  private readonly string _pluginsDirectoryName;
  private readonly string _devKitAssemblyName;
  private readonly Version _devKitMinSupportedVersion;
  private readonly string _defaultTextPluginAssemblyName;


  private readonly Dictionary<Guid, IPlugin> _plugins = [];
  public IReadOnlyDictionary<Guid, IPlugin> Plugins { get; }


  public async Task<PluginInstallationResult> LoadPluginAsync(ZipArchive zipArchive,
                                                              Guid pluginId,
                                                              Version pluginVersion,
                                                              IProgress<int>? progress = null,
                                                              CancellationToken cancellationToken = default)
  {
    var entriesCount = zipArchive.Entries.Count;
    if (entriesCount <= 0)
    {
      return PluginInstallationResult.EmptyArchive;
    }
    if (entriesCount > 10000)
    {
      return PluginInstallationResult.ExceededArchiveEntriesCount;
    }

    var destinationPluginDirectory = Path.Combine(_pluginsDirectoryName, pluginId.ToString());
    try
    {
      var pluginInstallationResult = await Task.Run(() =>
      {
        var result = PluginInstallationResult.Success;
        var totalUncompressedArchiveSize = 0L;
        for (var i = 0; i < entriesCount; i++)
        {
          cancellationToken.ThrowIfCancellationRequested();
          // todo: test with:
          // throw new OperationCanceledException();
          var entry = zipArchive.Entries[i];
          var entryFullName = entry.FullName;

          var directory = Path.GetDirectoryName(entryFullName);
          var fileName = Path.GetFileName(entryFullName);
          if (directory is not null && fileName is not null)
          {
            var destinationPath = Path.Combine(destinationPluginDirectory, pluginVersion.ToString(), directory, fileName);
            try
            {
              entry.ExtractToFile(destinationPath, true);
            }
            catch (IOException)
            {
              // should be ignored in case the plugin is installed immediately after uninstallation
              // and the plugins files are not removed yet
            }

            var uncompressedFileSize = new FileInfo(destinationPath).Length;
            var compressionRatio = (double) uncompressedFileSize / entry.CompressedLength;
            totalUncompressedArchiveSize += uncompressedFileSize;
            var isCompressionRatioViolation = compressionRatio > 10;
            var isTotalUncompressedArchiveSizeViolation = totalUncompressedArchiveSize > 1024 * 1024 * 1024; // 1 GB
            if (isCompressionRatioViolation || isTotalUncompressedArchiveSizeViolation)
            {
              Directory.Delete(destinationPluginDirectory, true);
              if (isCompressionRatioViolation)
              {
                return PluginInstallationResult.AbnormalArchiveCompressionRatio;
              }
              if (isTotalUncompressedArchiveSizeViolation)
              {
                return PluginInstallationResult.ExceededUncompressedArchiveSize;
              }
            }
          }

          progress?.Report((int) ((double) i / entriesCount * 100));
        }
        return result;
      }, cancellationToken).ConfigureAwait(false);
      if (pluginInstallationResult != PluginInstallationResult.Success)
      {
        return pluginInstallationResult;
      }
    }
    catch (OperationCanceledException)
    {
      // delete already extracted files
      Directory.Delete(destinationPluginDirectory, true);
      return PluginInstallationResult.CancelledByUser;
    }


    var result = PluginInstallationResult.Success;
    if (await _pluginRepository.ExistsAsync(pluginId, pluginVersion).ConfigureAwait(false))
    {
      await _pluginRepository.UpdateIsInstalledAsync(pluginId, true).ConfigureAwait(false);
    }
    else if (await _pluginRepository.ExistsAsync(pluginId).ConfigureAwait(false))
    {
      await _pluginRepository.UpdateAsync(pluginId, pluginVersion, true).ConfigureAwait(false);
    }
    else
    {
      result = await LoadPluginModuleAsync(
        new DirectoryInfo(destinationPluginDirectory),
        _appDomain.GetLoadedAssemblies()
      ).ConfigureAwait(false);
    }

    return result;
  }


  public async Task<PluginInstallationResult> LoadPluginModuleAsync(DirectoryInfo pluginDirectory,
                                                                    Assembly[]? alreadyLoadedAssemblies = null)
  {
    var files = pluginDirectory
      .GetFiles("*.dll")
      .Where(file => !IsAssemblyFileAlreadyLoaded(file, alreadyLoadedAssemblies ?? _appDomain.GetLoadedAssemblies()));
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

    var result = await LoadPluginModuleFromLoadedAssembliesAsync(pluginDirectory, loadedAssemblies).ConfigureAwait(false);

    if (result != PluginInstallationResult.Success)
    {
      Directory.Delete(pluginDirectory.FullName, true);
      return result;
    }

    return result;
  }


  private async Task<PluginInstallationResult> LoadPluginModuleFromLoadedAssembliesAsync(DirectoryInfo pluginDirectory,
                                                                                         List<Assembly> loadedAssemblies)
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
      return PluginInstallationResult.Incompatibility;
    }

    try
    {
      var pluginModuleType = pluginAssembly
        .GetExportedTypes()
        .FirstOrDefault(t => typeof(IPluginModule).IsAssignableFrom(t)
                             && t != typeof(IPluginModule)
                             && !t.IsAbstract);
      if (pluginModuleType is null)
      {
        return PluginInstallationResult.MissingPluginModuleType;
      }

      var pluginModule = (IPluginModule?) Activator.CreateInstance(pluginModuleType);
      if (pluginModule is null)
      {
        return PluginInstallationResult.PluginModuleInstanceCreationProblem;
      }
      var plugin = await ConstructPluginAsync(pluginModule).ConfigureAwait(false);
      var pluginId = pluginModule.Id;
      _plugins[pluginId] = plugin;

      if (!await _pluginRepository.ExistsAsync(pluginId).ConfigureAwait(false))
      {
        await _pluginRepository.AddAsync(new()
        {
          Id = pluginId,
          Name = pluginModule.Name,
          Version = pluginModule.Version.ToString(),
          Author = pluginModule.Author,
          Description = pluginModule.Description,
          FilesDirectory = pluginDirectory.FullName
        }).ConfigureAwait(false);
      }

      return PluginInstallationResult.Success;
    }
    catch (TypeLoadException e)
    {
      _sentryHub.CaptureException(e);
      return PluginInstallationResult.TypesLoadingProblem;
    }
    catch (Exception e)
    {
      _sentryHub.CaptureException(e);
      return PluginInstallationResult.OtherProblem;
    }
  }


  private static bool IsAssemblyFileAlreadyLoaded(FileInfo file, Assembly[] alreadyLoadedAssemblies)
  {
    return alreadyLoadedAssemblies.Any(
      assembly => string.Equals(Path.GetFileName(assembly.Location), file.Name, StringComparison.OrdinalIgnoreCase)
    );
  }


  private async Task<IPlugin> ConstructPluginAsync(IPluginModule pluginModule)
  {
    await _joinableTaskFactory.SwitchToMainThreadAsync();
    pluginModule.RegisterTypes(_containerExtension);
    pluginModule.OnInitialized(_containerExtension);
    return _containerExtension.Resolve<IPlugin>(pluginModule.Id.ToString());
  }
}
