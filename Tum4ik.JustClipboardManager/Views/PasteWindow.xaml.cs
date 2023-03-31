using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tum4ik.JustClipboardManager.ViewModels;

namespace Tum4ik.JustClipboardManager.Views;

/// <summary>
/// Interaction logic for PasteWindow.xaml
/// </summary>
public partial class PasteWindow
{
  private PasteWindowViewModel _vm = null!;

  public PasteWindow()
  {
    InitializeComponent();
    DataContextChanged += (s, e) => _vm = (PasteWindowViewModel) e.NewValue;
  }


  private static readonly object s_locker = new();
  private bool _isLoading;
  private ScrollViewer? _scrollViewer;


  private void This_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    if (!(bool) e.NewValue)
    {
      _scrollViewer ??= FindVisualChild<ScrollViewer>(_listBox);
      _scrollViewer?.ScrollToHome();
    }
  }


  private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
  {
    lock (s_locker)
    {
      if (_isLoading)
      {
        return;
      }

      _isLoading = true;
    }

    if (e.ExtentHeight != default
        && e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight)
    {
      _vm.LoadNextClipsBatchAsync().ContinueWith(t => _isLoading = false, TaskScheduler.Default).Await(e => throw e);
    }
    else
    {
      _isLoading = false;
    }
  }


  private static T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
  {
    var childrenCount = VisualTreeHelper.GetChildrenCount(obj);
    for (var i = 0; i < childrenCount; i++)
    {
      var child = VisualTreeHelper.GetChild(obj, i);
      if (child is T)
      {
        return (T) child;
      }
      else
      {
        var childOfChild = FindVisualChild<T>(child);
        if (childOfChild is not null)
        {
          return childOfChild;
        }
      }
    }
    return null;
  }
}
