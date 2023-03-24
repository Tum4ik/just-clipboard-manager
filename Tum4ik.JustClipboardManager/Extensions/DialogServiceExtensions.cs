using Prism.Services.Dialogs;

namespace Tum4ik.JustClipboardManager.Extensions;
internal static class DialogServiceExtensions
{
  public static void Show(this IDialogService dialogService, string name, IDialogParameters parameters)
  {
    dialogService.Show(name, parameters, r => { });
  }


  public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters)
  {
    dialogService.ShowDialog(name, parameters, r => { });
  }
}
