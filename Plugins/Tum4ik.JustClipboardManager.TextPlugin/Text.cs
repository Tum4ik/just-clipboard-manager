using System.Text;
using System.Windows;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.TextPlugin;

[Plugin("D930D2CD-3FD9-4012-A363-120676E22AFA")]
public sealed class Text : Plugin<TextVisualTree>
{
  public override string Format { get; } = DataFormats.UnicodeText;


  public override ClipData? ProcessData(object data)
  {
    var text = data as string;
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


  public override object RestoreData(byte[] bytes)
  {
    return Encoding.UTF8.GetString(bytes);
  }


  public override object RestoreRepresentationData(byte[] bytes)
  {
    return Encoding.UTF8.GetString(bytes);
  }
}


public sealed class TextPlugin : PluginModule<Text> { }
