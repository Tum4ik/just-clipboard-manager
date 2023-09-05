using Prism.Ioc;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Extensions;
public static class ContainerProviderExtensions
{
  public static IPlugin ResolvePlugin(this IContainerProvider containerProvider, string pluginId)
  {
    return containerProvider.Resolve<IPlugin>(pluginId);
  }
}
