using Prism.Ioc;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Extensions;
public static class ContainerProviderExtensions
{
  public static IPlugin ResolvePlugin(this IContainerProvider containerProvider, Guid pluginId)
  {
    return containerProvider.Resolve<IPlugin>(pluginId.ToString());
  }
}
