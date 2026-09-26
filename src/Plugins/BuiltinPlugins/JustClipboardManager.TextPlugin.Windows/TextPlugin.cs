using System;
using System.Text;
using System.Threading.Tasks;
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
  public override async Task<ProcessDataResult> ProcessDataAsync(DataPackageView clipboardContent)
  {
    var text = (await clipboardContent.GetTextAsync()).Trim();
    var firstLine = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];
    return new()
    {
      FormattedDataResults = [new() { Data = Encoding.UTF8.GetBytes(text) }],
      PreviewData = Encoding.UTF8.GetBytes(firstLine),
      SearchLabel = text,
    };
  }


  public override DataPackage RestoreData(byte[] bytes)
  {
    throw new System.NotImplementedException();
  }
}
