using System.Windows.Automation;
using Tum4ik.JustClipboardManager.UiTests.Extensions;

namespace Tum4ik.JustClipboardManager.UiTests.Helpers;
internal static class AppActionsHelper
{
  public static void TrayIconRightClick()
  {
    var chevron = AutomationElement.RootElement.FindFirst(
      TreeScope.Descendants,
      new PropertyCondition(AutomationElement.NameProperty, "Show Hidden Icons")
    );
    chevron.Invoke();

    var notificationIconAreas = AutomationElement.RootElement.FindAll(
      TreeScope.Descendants,
      new PropertyCondition(AutomationElement.ClassNameProperty, "Windows.UI.Input.InputSite.WindowClass")
    );
    foreach (AutomationElement area in notificationIconAreas)
    {
      var icon = area
        .FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
        .Cast<AutomationElement>()
        .FirstOrDefault(ni =>
        {
          var name = (string) ni.GetCurrentPropertyValue(AutomationElement.NameProperty);
          return name.Contains("Just Clipboard Manager - UI Test", StringComparison.OrdinalIgnoreCase);
        });
      if (icon is not null)
      {
        icon.RightClick();
        return;
      }
    }
    throw new Exception("Icon is not found in tray.");
  }
}
