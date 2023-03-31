using System.Windows;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Services.PInvoke;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

namespace Tum4ik.JustClipboardManager.Services.Dialogs;

internal class ExtendedDialogService : DialogService, IDialogService
{
  private readonly IContainerExtension _containerExtension;
  private readonly IUser32DllService _user32Dll;

  public ExtendedDialogService(IContainerExtension containerExtension,
                               IUser32DllService user32Dll)
    : base(containerExtension)
  {
    _containerExtension = containerExtension;
    _user32Dll = user32Dll;
  }


  public new void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
  {
    ShowInternal(name, parameters, cb => base.Show(name, parameters, cb), callback);
  }

  public new void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback, string? windowName)
  {
    ShowInternal(name, parameters, cb => base.Show(name, parameters, cb, windowName), callback);
  }

  public new void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
  {
    ShowInternal(name, parameters, cb => base.ShowDialog(name, parameters, cb), callback);
  }

  public new void ShowDialog(string name,
                             IDialogParameters parameters,
                             Action<IDialogResult> callback,
                             string? windowName)
  {
    ShowInternal(name, parameters, cb => base.ShowDialog(name, parameters, cb, windowName), callback);
  }


  private static readonly Dictionary<string, IDialogWindow> s_openDialogs = new();


  protected override IDialogWindow CreateDialogWindow(string? name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      name = WindowNames.SimpleDialogWindow;
    }
    return _containerExtension.Resolve<IDialogWindow>(name);
  }


  protected override void ConfigureDialogWindowContent(string dialogName,
                                                       IDialogWindow window,
                                                       IDialogParameters parameters)
  {
    base.ConfigureDialogWindowContent(dialogName, window, parameters);
    if (SingleInstanceDialogsProvider.IsSingleInstanceDialog(dialogName))
    {
      s_openDialogs[dialogName] = window;
    }
  }


  private void ShowInternal(string name,
                            IDialogParameters parameters,
                            Action<Action<IDialogResult>> show,
                            Action<IDialogResult> originalCallback)
  {
    var finalCallback = originalCallback;
    if (SingleInstanceDialogsProvider.IsSingleInstanceDialog(name))
    {
      if (s_openDialogs.TryGetValue(name, out var dialogWindow)
          && dialogWindow.DataContext is IDialogAware viewModel)
      {
        viewModel.OnDialogOpened(parameters);
        if (dialogWindow is IDialogWindowExtended dialogWindowExtended)
        {
          if (dialogWindowExtended.WindowState == WindowState.Minimized)
          {
            _user32Dll.ShowWindow(dialogWindowExtended.Handle, ShowWindowCommand.SW_RESTORE);
          }
          else if (!dialogWindowExtended.IsActive)
          {
            dialogWindowExtended.Activate();
          }
        }

        return;
      }

      finalCallback = r =>
      {
        originalCallback.Invoke(r);
        s_openDialogs.Remove(name);
      };
    }

    show(finalCallback);
  }
}


internal static class SingleInstanceDialogsProvider
{
  private static readonly HashSet<string> s_singleInstanceDialogNames = new();

  public static void RegisterSingleInstanceDialog(string name)
  {
    s_singleInstanceDialogNames.Add(name);
  }

  public static bool IsSingleInstanceDialog(string name)
  {
    return s_singleInstanceDialogNames.Contains(name);
  }
}
