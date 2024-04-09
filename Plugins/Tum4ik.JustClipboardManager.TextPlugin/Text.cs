using System.Text;
using System.Windows;
using Prism.Modularity;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.TextPlugin;

[Plugin(
  Id = PluginId,
  Name = "Default Text Plugin",
  Version = "3.0.0",
  Author = "Yevheniy Tymchishin",
  AuthorEmail = "timchishinevgeniy@gmail.com",
  Description = "A simple plugin to deal with the text data"
)]
public sealed class Text : Plugin<TextVisualTree>
{
  internal const string PluginId = "D930D2CD-3FD9-4012-A363-120676E22AFA";

  public override IReadOnlyCollection<string> Formats { get; } = new[] { DataFormats.UnicodeText, DataFormats.Text };


  public override ClipData? ProcessData(IDataObject dataObject)
  {
    var text = dataObject?.GetData(DataFormats.UnicodeText) as string;
    if (string.IsNullOrWhiteSpace(text))
    {
      return null;
    }
    var bytes = Encoding.UTF8.GetBytes(text.Trim());
    return new()
    {
      Data = bytes,
      RepresentationData = bytes,
      SearchLabel = text
    };
  }


  public override object? RestoreData(byte[] bytes, string? additionalInfo)
  {
    return Encoding.UTF8.GetString(bytes);
  }


  public override object? RestoreRepresentationData(byte[] bytes, string? additionalInfo)
  {
    return Encoding.UTF8.GetString(bytes);
  }
}


[Module(ModuleName = Text.PluginId)]
public sealed class TextPlugin : PluginModule<Text> { }
