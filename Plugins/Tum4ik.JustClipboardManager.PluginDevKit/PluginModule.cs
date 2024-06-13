using System.Reflection;
using System.Resources;
using Prism.Ioc;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;
using Tum4ik.JustClipboardManager.PluginDevKit.Exceptions;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.PluginDevKit;

public abstract class PluginModule<TPlugin> : IPluginModule
  where TPlugin : IPlugin
{
  protected PluginModule()
  {
    var pluginAttribute = GetType().GetCustomAttribute<PluginAttribute>();
    if (pluginAttribute is null)
    {
      throw new PluginModuleException("Plugin attribute is missing.");
    }
    Id = Guid.Parse(pluginAttribute.Id);
    Name = pluginAttribute.Name;
    Version = Version.Parse(pluginAttribute.Version);
    Author = pluginAttribute.Author;
    Description = pluginAttribute.Description;
  }


  public Guid Id { get; }
  public string Name { get; }
  public Version Version { get; }
  public string? Author { get; }
  public string? Description { get; }


  public void RegisterTypes(IContainerRegistry containerRegistry)
  {
    var resourceManager = CreateResourceManager();
    if (resourceManager is not null)
    {
      containerRegistry.RegisterInstance(resourceManager);
      containerRegistry.RegisterSingleton<IPluginTranslationService, PluginTranslationService>();
    }

    containerRegistry.RegisterSingleton<IPlugin, TPlugin>(Id.ToString());
  }


  public void OnInitialized(IContainerProvider containerProvider)
  {
    //var pluginsService = containerProvider.Resolve<IPluginsRegistryService>();
    //var plugin = (TPlugin) containerProvider.Resolve<IPlugin>(Id.ToString());
    //plugin.Id = Id;
    //plugin.RepresentationDataDataTemplate = new() { VisualTree = new(typeof(TVisualTree)) };
    //pluginsService.RegisterPlugin(plugin);
  }


  protected virtual ResourceManager? CreateResourceManager() => null;
}
