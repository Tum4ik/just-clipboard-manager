namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;
public interface IPluginsService
{
  void Register(string id);
  void Unregister(string id);
  IReadOnlyCollection<IPlugin> Plugins();
  IPlugin? GetPlugin(string id);
}
