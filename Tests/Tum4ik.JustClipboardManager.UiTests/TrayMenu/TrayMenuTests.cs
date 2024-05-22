using System.Windows.Automation;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.UiTests.Extensions;
using Tum4ik.JustClipboardManager.UiTests.Helpers;

namespace Tum4ik.JustClipboardManager.UiTests.TrayMenu;

[Collection(Collections.Application)]
public class TrayMenuTests
{
  [Fact]
  internal void SettingsClickTest()
  {
    AppActionsHelper.TrayIconRightClick();
    var contextMenuElement = AutomationElement.RootElement.FindFirst(
      TreeScope.Descendants,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.TrayIconContextMenu)
    );
    var settingsButtonElement = contextMenuElement.FindFirst(
      TreeScope.Children,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.TrayIconContextMenuSettings)
    );
    settingsButtonElement.Invoke();

    var mainWindowElement = AutomationElement.RootElement.FindFirst(
      TreeScope.Children,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.MainWindow)
    );
    var mainWindowSettingsButtonElement = mainWindowElement.FindFirst(
      TreeScope.Descendants,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.MainWindowSettingsButton)
    );
    var mainWindowSettingsButtonPattern = (SelectionItemPattern) mainWindowSettingsButtonElement.GetCurrentPattern(SelectionItemPattern.Pattern);
    mainWindowSettingsButtonPattern.Current.IsSelected.Should().BeTrue();
  }
}
