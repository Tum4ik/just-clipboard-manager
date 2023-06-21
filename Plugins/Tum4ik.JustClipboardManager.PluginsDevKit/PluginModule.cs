using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.PluginDevKit;

public abstract class PluginModule<T> : IModule
  where T : IPlugin
{
  protected PluginModule()
  {
    _pluginId = Plugin<FrameworkElement>.GetId(typeof(T));
  }


  private readonly Guid? _pluginId;


  public void RegisterTypes(IContainerRegistry containerRegistry)
  {
    if (!_pluginId.HasValue)
    {
      return;
    }

    containerRegistry.RegisterSingleton<IPlugin, T>(_pluginId.ToString());
  }


  public void OnInitialized(IContainerProvider containerProvider)
  {
    if (!_pluginId.HasValue)
    {
      return;
    }

    var pluginsService = containerProvider.Resolve<IPluginsService>();
    pluginsService.Register(_pluginId.Value);
  }
}
