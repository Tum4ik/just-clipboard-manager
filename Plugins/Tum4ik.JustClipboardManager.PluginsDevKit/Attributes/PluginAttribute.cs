namespace Tum4ik.JustClipboardManager.PluginDevKit.Attributes;

/// <summary>
/// Indicates that the class should be considered a plugin.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PluginAttribute : Attribute
{
  public PluginAttribute(string id)
  {
    Id = Guid.Parse(id);
  }

  /// <summary>
  /// The unique id of the plugin.
  /// </summary>
  public Guid Id { get; }
}
