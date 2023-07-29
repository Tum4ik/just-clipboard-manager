using Prism.Events;
using Prism.Ioc;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Extensions;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

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


  public void Register(string id)
  {
    var plugin = _containerProvider.ResolvePlugin(id);
    _plugins[id]=plugin;
    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  public void Unregister(string id)
  {
    _plugins.Remove(id);
    _eventAggregator.GetEvent<PluginsChainUpdatedEvent>().Publish();
  }


  public IReadOnlyCollection<IPlugin> Plugins()
  {
    return _plugins.Values;
  }


  public IPlugin? GetPlugin(string id)
  {
    if (_plugins.TryGetValue(id, out var plugin))
    {
      return plugin;
    }
    return null;
  }
}
