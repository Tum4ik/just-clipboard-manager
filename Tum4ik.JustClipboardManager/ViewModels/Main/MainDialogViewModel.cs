using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.ViewModels.Main;

[INotifyPropertyChanged]
internal partial class MainDialogViewModel : IDialogAware
{
  public MainDialogViewModel(IInfoService infoService)
  {
    Title = infoService.GetProductName();
  }


  public bool CanCloseDialog()
  {
    return true;
  }

  public void OnDialogClosed()
  {
  }

  public void OnDialogOpened(IDialogParameters parameters)
  {
  }

  public string Title { get; }
  public string? Tag { get; }
#if DEBUG
  = "Development";
#endif

  public event Action<IDialogResult>? RequestClose;


  [ObservableProperty]
  private WindowState _windowState;


  [RelayCommand]
  private void Minimize()
  {
    WindowState = WindowState.Minimized;
  }


  [RelayCommand]
  private void MaximizeRestore()
  {
    if (WindowState == WindowState.Normal)
    {
      WindowState = WindowState.Maximized;
    }
    else if (WindowState == WindowState.Maximized)
    {
      WindowState = WindowState.Normal;
    }
  }


  [RelayCommand]
  private void Close()
  {
    RequestClose?.Invoke(new DialogResult());
  }
}
