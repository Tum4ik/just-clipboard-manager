using Tum4ik.JustClipboardManager.ViewModels;

namespace Tum4ik.JustClipboardManager.Views;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class PasteWindow : Window
{
  public PasteWindowViewModel Vm { get; }

  public PasteWindow(PasteWindowViewModel vm)
  {
    Vm = vm;
    InitializeComponent();
  }
}
