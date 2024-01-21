namespace Tum4ik.JustClipboardManager.PluginDevKit.Models;

/// <summary>
/// Stores the data and other info of the clip.
/// </summary>
public sealed class ClipData
{
  /// <summary>
  /// The valuable data of the clip.
  /// </summary>
  public required byte[] Data { get; init; }

  /// <summary>
  /// The data will be used for the preview in the paste window.
  /// </summary>
  public required byte[] RepresentationData { get; init; }

  /// <summary>
  /// The label will be used during the search process in the paste window.
  /// </summary>
  public string? SearchLabel { get; init; }

  /// <summary>
  /// Any additional info you need for the restore data process.
  /// </summary>
  public string? AdditionalInfo { get; init; }
}
