using System;
using System.Windows.Interop;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager.Services;
internal class PasteWindowService : IPasteWindowService
{
  private readonly PasteWindow _pasteWindow;

  public PasteWindowService(PasteWindow pasteWindow)
  {
    _pasteWindow = pasteWindow;
  }


  public IntPtr GetWindowHandle()
  {
    return new WindowInteropHelper(_pasteWindow).EnsureHandle();
  }


  public void ShowWindow()
  {
    _pasteWindow.Show();
  }
}
