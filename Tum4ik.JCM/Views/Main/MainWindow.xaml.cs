using System.Diagnostics;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using Tum4ik.JustClipboardManager.ViewModels.Main;
using Windows.Win32.Foundation;
using WinRT.Interop;
using static Windows.Win32.PInvoke;

namespace Tum4ik.JustClipboardManager.Views.Main;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class MainWindow : Window
{
  private readonly IUser32Dll _user32Dll;

  public MainWindowViewModel Vm { get; }

  public MainWindow(MainWindowViewModel vm,
                    IUser32Dll user32Dll)
  {
    Vm = vm;
    _user32Dll = user32Dll;

    ExtendsContentIntoTitleBar = true;
    SetTaskbarIcon();
    InitializeComponent();
  }


  private void SetTaskbarIcon()
  {
    var hWnd = WindowNative.GetWindowHandle(this);
    var exeFileName = Process.GetCurrentProcess().MainModule?.FileName;
    if (exeFileName is not null)
    {
      var ico = System.Drawing.Icon.ExtractAssociatedIcon(exeFileName);
      if (ico is not null)
      {
        _user32Dll.SendMessage((HWND) hWnd, WM_SETICON, ICON_BIG, ico.Handle);
      }
    }
  }
}
