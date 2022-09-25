using System.Windows.Interop;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager.Services;
internal class PasteWindowService : IPasteWindowService
{
  private readonly PasteWindow _pasteWindow;

  public PasteWindowService(PasteWindow pasteWindow)
  {
    _pasteWindow = pasteWindow;
    WindowHandle = new WindowInteropHelper(_pasteWindow).EnsureHandle();
  }


  public IntPtr WindowHandle { get; }


  public void ShowWindow()
  {
    _pasteWindow.Show();
    _pasteWindow.Activate();
  }
}
