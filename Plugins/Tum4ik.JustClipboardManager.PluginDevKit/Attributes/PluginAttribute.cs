namespace Tum4ik.JustClipboardManager.PluginDevKit.Attributes;

/// <summary>
/// Indicates that the class should be considered a plugin.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PluginAttribute : Attribute
{
  /// <summary>
  /// The unique id of the plugin (required).
  /// </summary>
  public required string Id { get; init; }

  /// <summary>
  /// The name of the plugin to be displayed for users (required).
  /// </summary>
  public required string Name { get; init; }

  /// <summary>
  /// The version of the plugin (required).
  /// </summary>
  public required string Version { get; init; }

  /// <summary>
  /// The author name to be displayed for users (optional).
  /// </summary>
  public string? Author { get; init; }

  /// <summary>
  /// The author e-mail to be displayed for users (optional).
  /// </summary>
  public string? AuthorEmail { get; init; }

  /// <summary>
  /// The description of the plugin (optional).
  /// </summary>
  public string? Description { get; init; }
}
