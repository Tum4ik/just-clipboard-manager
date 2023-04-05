using System.Windows.Media;
using System.Windows;

namespace Tum4ik.JustClipboardManager.Extensions;

internal static class DependencyObjectExtensions
{
  public static T? FindVisualChild<T>(this DependencyObject obj) where T : DependencyObject
  {
    var childrenCount = VisualTreeHelper.GetChildrenCount(obj);
    for (var i = 0; i < childrenCount; i++)
    {
      var child = VisualTreeHelper.GetChild(obj, i);
      if (child is T theChild)
      {
        return theChild;
      }

      var childOfChild = child.FindVisualChild<T>();
      if (childOfChild is not null)
      {
        return childOfChild;
      }
    }

    return null;
  }
}
