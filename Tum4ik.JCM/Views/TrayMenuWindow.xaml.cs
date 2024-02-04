using System.Windows.Interop;
using Microsoft.UI.Windowing;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using Tum4ik.JustClipboardManager.ViewModels;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using WinRT.Interop;
using static Windows.Win32.PInvoke;

namespace Tum4ik.JustClipboardManager.Views;

internal sealed partial class TrayMenuWindow : Window
{
  private const int MenuMinWidth = 164;
  private const int MenuMinHeight = 44;

  private readonly Func<IUser32Dll> _user32Dll;
  //private readonly IComctl32Dll _comctl32Dll;

  public TrayMenuWindowViewModel Vm { get; }

  public TrayMenuWindow(TrayMenuWindowViewModel vm,
                        Func<IUser32Dll> user32Dll/*,
                        IComctl32Dll comctl32Dll*/)
  {
    Vm = vm;
    _user32Dll = user32Dll;
    //_comctl32Dll = comctl32Dll;
    SetupWindow();
    InitializeComponent();
    
    _windowHandle = WindowNative.GetWindowHandle(this);
    //comctl32Dll.SetWindowSubclass((HWND) _windowHandle, HwndHook, 0, 0);
  }


  private void Window_Activated(object sender, WindowActivatedEventArgs args)
  {
    if (args.WindowActivationState == WindowActivationState.Deactivated)
    {
      AppWindow.Hide();
    }
    else if (args.WindowActivationState == WindowActivationState.CodeActivated)
    {
      SetWindowSizeAndPosition();
      // Workaround to activate (bring to foreground) window next time. Othewise the window appears inactive.
      // https://github.com/microsoft/microsoft-ui-xaml/issues/7595
      _user32Dll().SetForegroundWindow((HWND) _windowHandle);
    }
  }


  private readonly nint _windowHandle;


  private void SetupWindow()
  {
    AppWindow.IsShownInSwitchers = false;
    AppWindow.Resize(new(MenuMinWidth, MenuMinHeight));
    var overlappedPresenter = ((OverlappedPresenter) AppWindow.Presenter);
    overlappedPresenter.IsAlwaysOnTop = true;
    overlappedPresenter.IsMaximizable = false;
    overlappedPresenter.IsMinimizable = false;
    overlappedPresenter.IsResizable = false;
    overlappedPresenter.SetBorderAndTitleBar(false, false);
  }


  private void SetWindowSizeAndPosition()
  {
    _rootStackPanel.Measure(new(800, 600));
    var desiredSize = _rootStackPanel.DesiredSize;
    var targetWidth = desiredSize.Width > MenuMinWidth ? desiredSize.Width : MenuMinWidth;
    var targetHeight = desiredSize.Height > MenuMinHeight ? desiredSize.Height : MenuMinHeight;
    AppWindow.Resize(new((int) Math.Ceiling(targetWidth), (int) Math.Ceiling(targetHeight)));

    if (_user32Dll().GetCursorPos(out var position))
    {
      AppWindow.Move(new(position.X, position.Y - (int) targetHeight));
    }
  }


  /*/// <inheritdoc cref="HwndSourceHook"/>
  private LRESULT HwndHook(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
  {
    switch (uMsg)
    {
      case WM_ACTIVATE:
        switch ((uint) wParam)
        {
          case WA_INACTIVE:
            //_user32Dll.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_HIDE);
            break;
          case WA_ACTIVE:
            _user32Dll.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_SHOWNORMAL);
            //AppWindow.Show(false);
            break;
        }
        break;
    }

    return _comctl32Dll.DefSubclassProc(hWnd, uMsg, wParam, lParam);
  }*/


}
