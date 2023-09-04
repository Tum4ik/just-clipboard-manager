using Prism.Events;
using Prism.Ioc;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Extensions;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Properties;

namespace Tum4ik.JustClipboardManager.Services;
internal class PluginsService : IPluginsService
{
  private readonly IContainerProvider _containerProvider;
  private readonly IEventAggregator _eventAggregator;

  public PluginsService(IContainerProvider containerProvider,
                        IEventAggregator eventAggregator)
  {
    _containerProvider = containerProvider;
    _eventAggregator = eventAggregator;
  }


  private readonly Dictionary<string, IPlugin> _plugins = new();
  private readonly Dictionary<string, bool> _enabledPlugins = new();


  public IReadOnlyCollection<IPlugin> InstalledPlugins => _plugins.Values;


  public void RegisterPlugin(string id)
  {
    _plugins[id] = _containerProvider.ResolvePlugin(id);
    _enabledPlugins[id] = PluginSettings.Default.Get(id, true);
    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  public void UnregisterPlugin(string id)
  {
    _plugins.Remove(id);
    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  public IPlugin? GetPlugin(string id)
  {
    if (_plugins.TryGetValue(id, out var plugin))
    {
      return plugin;
    }
    return null;
  }


  public void EnablePlugin(string id)
  {
    if (_enabledPlugins.TryGetValue(id, out var enabled) && !enabled)
    {
      _enabledPlugins[id] = true;
      PluginSettings.Default.Save(id, true);
    }
  }


  public void DisablePlugin(string id)
  {
    if (_enabledPlugins.TryGetValue(id, out var enabled) && enabled)
    {
      _enabledPlugins[id] = false;
      PluginSettings.Default.Save(id, false);
    }
  }


  public bool IsPluginInstalled(string id)
  {
    return _plugins.ContainsKey(id);
  }


  public bool IsPluginEnabled(string id)
  {
    if (_enabledPlugins.TryGetValue(id, out var enabled))
    {
      return enabled;
    }
    return false;
  }
}
