using System.Windows.Interop;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.Dialogs;

namespace Tum4ik.JustClipboardManager.Views.Main;

/// <summary>
/// Interaction logic for MainDialogWindow.xaml
/// </summary>
internal partial class MainDialogWindow : IDialogWindowExtended
{
  public MainDialogWindow()
  {
    InitializeComponent();
  }


  private nint? _handle;
  public nint Handle => _handle ??= new WindowInteropHelper(this).EnsureHandle();


  public IDialogResult? Result { get; set; }
}
