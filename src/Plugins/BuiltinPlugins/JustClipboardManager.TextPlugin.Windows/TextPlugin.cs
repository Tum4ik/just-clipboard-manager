using JustClipboardManager.PluginDevKit.Core;
using JustClipboardManager.PluginDevKit.Windows;
using Windows.ApplicationModel.DataTransfer;

namespace JustClipboardManager.TextPlugin.Windows;

[Plugin(
  Id = "b50fb996-904f-4391-94a6-78d95479def1",
  Name = "Text Plugin",
  Version = "5.0.0"
)]
public class TextPlugin : JcmPlugin
{
  public override object? ProcessData(DataPackageView dataPackageView)
  {
    throw new System.NotImplementedException();
  }
}
