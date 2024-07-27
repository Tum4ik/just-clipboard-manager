using System.Windows.Automation;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Tum4ik.JustClipboardManager.UiTests.Extensions;
internal static class AutomationElementExtensions
{
  public static void Invoke(this AutomationElement element)
  {
    var invokePattern = (InvokePattern) element.GetCurrentPattern(InvokePattern.Pattern);
    invokePattern.Invoke();
  }


  public unsafe static void RightClick(this AutomationElement element)
  {
    var boundingRect = element.Current.BoundingRectangle;
    
    var centerX = boundingRect.X + boundingRect.Width / 2;
    var centerY = boundingRect.Y + boundingRect.Height / 2;

    var inputs = new INPUT[3];
    inputs[0].type = inputs[1].type = inputs[2].type = INPUT_TYPE.INPUT_MOUSE;

    inputs[0].Anonymous.mi.dx = (int) ((centerX * ushort.MaxValue) / PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSCREEN));
    inputs[0].Anonymous.mi.dy = (int) ((centerY * ushort.MaxValue) / PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSCREEN));
    inputs[0].Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE;

    inputs[1].Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN;
    inputs[2].Anonymous.mi.dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP;

    PInvoke.SendInput(inputs, sizeof(INPUT));
    Thread.Sleep(1000);
  }
}
