using System.Windows;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.PluginDevKit;
public interface IPlugin
{
  string? Id { get; }

  string Format { get; }

  DataTemplate RepresentationDataDataTemplate { get; }

  ClipData? ProcessData(object data);

  object? RestoreData(byte[] bytes, string? additionalInfo);

  object? RestoreRepresentationData(byte[] bytes, string? additionalInfo);
}
