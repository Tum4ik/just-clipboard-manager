using System;
using JustClipboardManager.Application.Services;

namespace JustClipboardManager.Services;

internal class ClipboardService : IClipboardService
{
  public void Initialize()
  {
    Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged += async (s, e) =>
    {
      var view = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
      var data = await view.GetTextAsync();
    };
  }
}
