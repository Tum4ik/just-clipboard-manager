namespace Tum4ik.JustClipboardManager.Plugins;
internal interface IPluginLoader
{
  /// <summary>
  /// Loads plugins assemblies from the Plugins folder, retrieves plugins types and registers them in the IoC container.
  /// </summary>
  void Load();
}
