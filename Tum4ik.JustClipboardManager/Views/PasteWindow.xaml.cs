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


  private readonly SemaphoreSlim _semaphore = new(1, 1);
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


  private async void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
  {
    try
    {
      await _semaphore.WaitAsync();
      if (_isLoading)
      {
        return;
      }

      _isLoading = true;
    }
    finally
    {
      _semaphore.Release();
    }

    if (e.ExtentHeight != default
        && e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight)
    {
      await _vm.LoadNextClipsBatchAsync();
    }

    _isLoading = false;
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
