using Microsoft.UI.Xaml.Controls;

namespace JustClipboardManager;

public interface IAppWindow
{
  WebView2 WebView { get; }
}
