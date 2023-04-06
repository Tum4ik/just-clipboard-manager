using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace Tum4ik.JustClipboardManager.Styles;

public partial class GeneralStyles
{
  private void Hyperlink_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
  {
    var hyperlink = (Hyperlink) sender;
    var storyboard = Application.Current.Resources["HyperlinkPressedForegroundStoryboard"] as Storyboard;
    storyboard?.Begin(hyperlink);
  }
}
