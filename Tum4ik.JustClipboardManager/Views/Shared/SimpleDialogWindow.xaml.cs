using Prism.Services.Dialogs;

namespace Tum4ik.JustClipboardManager.Views.Shared;
/// <summary>
/// Interaction logic for SimpleDialogWindow.xaml
/// </summary>
public partial class SimpleDialogWindow : IDialogWindow
{
  public SimpleDialogWindow()
  {
    InitializeComponent();
  }


  public IDialogResult Result { get; set; }
}
