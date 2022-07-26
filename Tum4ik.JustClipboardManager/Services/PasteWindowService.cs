using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Tum4ik.JustClipboardManager.ViewModels;
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


  private TaskCompletionSource<PasteWindowResult?>? _pasteWindowResultSource;

  public Task<PasteWindowResult?> ShowWindowAsync(IntPtr targetWindowToPaste)
  {
    _pasteWindow.IsVisibleChanged += PasteWindow_IsVisibleChanged;
    _pasteWindow.Show();
    _pasteWindowResultSource = new();
    return _pasteWindowResultSource.Task;
  }


  private void PasteWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    if (!(bool) e.NewValue)
    {
      _pasteWindow.IsVisibleChanged -= PasteWindow_IsVisibleChanged;
      var vm = (PasteWindowViewModel) _pasteWindow.DataContext;
      _pasteWindowResultSource?.SetResult(vm.PasteResult);
    }
  }
}
