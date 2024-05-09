using System.Reflection;
using System.Resources;
using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.PluginDevKit;

public abstract class PluginModule<TPlugin, TVisualTree> : IModule
  where TPlugin : Plugin
  where TVisualTree : FrameworkElement
{
  private readonly Guid _pluginId;

  protected PluginModule()
  {
    _ = Guid.TryParse(GetType().GetCustomAttribute<PluginAttribute>()?.Id, out _pluginId);
  }


  public void RegisterTypes(IContainerRegistry containerRegistry)
  {
    if (_pluginId == Guid.Empty)
    {
      return;
    }

    var resourceManager = CreateResourceManager();
    if (resourceManager is not null)
    {
      containerRegistry.RegisterInstance(resourceManager);
      containerRegistry.RegisterSingleton<IPluginTranslationService, PluginTranslationService>();
    }

    containerRegistry.RegisterSingleton<IPlugin, TPlugin>(_pluginId.ToString());
  }


  public void OnInitialized(IContainerProvider containerProvider)
  {
    if (_pluginId == Guid.Empty)
    {
      return;
    }

    var pluginsService = containerProvider.Resolve<IPluginsRegistryService>();
    var plugin = (TPlugin) containerProvider.Resolve<IPlugin>(_pluginId.ToString());
    plugin.Id = _pluginId;
    plugin.RepresentationDataDataTemplate = new() { VisualTree = new(typeof(TVisualTree)) };
    pluginsService.RegisterPlugin(plugin);
  }


  public virtual ResourceManager? CreateResourceManager() => null;
}
