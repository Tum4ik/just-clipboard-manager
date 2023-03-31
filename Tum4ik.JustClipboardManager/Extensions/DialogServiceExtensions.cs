using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;

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


  public static void ShowMainAppDialog(this IDialogService dialogService, IDialogParameters parameters)
  {
    dialogService.Show(DialogNames.MainDialog, parameters, r => { }, WindowNames.MainAppWindow);
  }
}
