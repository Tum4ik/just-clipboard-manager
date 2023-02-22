using System.Windows;
using Prism.Services.Dialogs;

namespace Tum4ik.JustClipboardManager.Services.Dialogs;
internal interface IDialogWindowExtended : IDialogWindow
{
  bool IsActive { get; }
  bool Activate();
  WindowState WindowState { get; set; }

  nint Handle { get; }
}
