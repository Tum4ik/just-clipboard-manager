namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;
public interface IPluginsService
{
  IReadOnlyCollection<IPlugin> Plugins { get; }
  void RegisterPlugin(string id);
  void UnregisterPlugin(string id);
  IPlugin? GetPlugin(string id);
  void EnablePlugin(string id);
  void DisablePlugin(string id);
  bool IsPluginEnabled(string id);
}
