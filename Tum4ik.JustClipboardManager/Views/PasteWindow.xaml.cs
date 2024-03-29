using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.ViewModels;

namespace Tum4ik.JustClipboardManager.Views;

/// <summary>
/// Interaction logic for PasteWindow.xaml
/// </summary>
internal partial class PasteWindow
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
  private Border? _windowBorder;


  private nint? _handle;
  public nint Handle => _handle ??= new WindowInteropHelper(this).EnsureHandle();


  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    _windowBorder = (Border) GetTemplateChild("_windowBorder");
  }


  internal Task<PasteWindowResult?> WaitForInputResultAsync()
  {
    return _vm.WaitForInputAsync();
  }


  private void This_Activated(object sender, EventArgs e)
  {
    _searchBox.Focus();
  }


  private void This_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    if (!(bool) e.NewValue)
    {
      _scrollViewer ??= _listBox.FindVisualChild<ScrollViewer>();
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


  private void Thumb_VerticalDragDelta(object sender, DragDeltaEventArgs e)
  {
    if (_windowBorder is null)
    {
      return;
    }
    var newHeight = (int) (_windowBorder.ActualHeight + e.VerticalChange);
    if (newHeight > 0)
    {
      _windowBorder.Height = newHeight;
    }
  }


  private void Thumb_HorizontalDragDelta(object sender, DragDeltaEventArgs e)
  {
    if (_windowBorder is null)
    {
      return;
    }
    var newWidth = (int) (_windowBorder.ActualWidth + e.HorizontalChange);
    if (newWidth > 0)
    {
      _windowBorder.Width = newWidth;
    }
  }
}
