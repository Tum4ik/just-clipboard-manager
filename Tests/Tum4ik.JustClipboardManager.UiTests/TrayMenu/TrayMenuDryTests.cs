using System.Windows.Automation;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.UiTests.Extensions;

namespace Tum4ik.JustClipboardManager.UiTests.TrayMenu;

/// <summary>
/// The tests are performed when no other windows of the application are shown (for example MainWindow is not open).
/// </summary>
[Collection(Collections.Application)]
public sealed class TrayMenuDryTests : IDisposable
{
  private readonly AutomationElement _contextMenuElement;

  public TrayMenuDryTests(ApplicationFixture applicationFixture)
  {
    var trayIconElement = applicationFixture.TrayIconElement;
    trayIconElement.RightClick();
    _contextMenuElement = AutomationElement.RootElement.FindFirst(
      TreeScope.Descendants,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.TrayIconContextMenu)
    );
  }


  public void Dispose()
  {
    var mainWindowElement = GetMainWindowElement();
    var mainWindowPattern = (WindowPattern) mainWindowElement.GetCurrentPattern(WindowPattern.Pattern);
    mainWindowPattern.Close();
  }


  [Fact]
  internal void SettingsClickTest()
  {
    var settingsButtonElement = _contextMenuElement.FindFirst(
      TreeScope.Children,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.TrayIconContextMenuSettings)
    );
    settingsButtonElement.Invoke();

    var mainWindowElement = GetMainWindowElement();
    var mainWindowSettingsButtonElement = mainWindowElement.FindFirst(
      TreeScope.Descendants,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.MainWindowSettingsButton)
    );
    var mainWindowSettingsViewElement = mainWindowElement.FindFirst(
      TreeScope.Descendants,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.MainWindowSettingsView)
    );

    var mainWindowSettingsButtonPattern = (SelectionItemPattern) mainWindowSettingsButtonElement.GetCurrentPattern(SelectionItemPattern.Pattern);
    mainWindowSettingsButtonPattern.Current.IsSelected.Should().BeTrue();

    mainWindowSettingsViewElement.Current.IsOffscreen.Should().BeFalse();
  }


  [Fact]
  internal void AboutClickTest()
  {
    var aboutButtonElement = _contextMenuElement.FindFirst(
      TreeScope.Children,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.TrayIconContextMenuAbout)
    );
    aboutButtonElement.Invoke();

    var mainWindowElement = GetMainWindowElement();
    var mainWindowAboutButtonElement = mainWindowElement.FindFirst(
      TreeScope.Descendants,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.MainWindowAboutButton)
    );
    var mainWindowAboutViewElement = mainWindowElement.FindFirst(
      TreeScope.Descendants,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.MainWindowAboutView)
    );

    var mainWindowAboutButtonPattern = (SelectionItemPattern) mainWindowAboutButtonElement.GetCurrentPattern(SelectionItemPattern.Pattern);
    mainWindowAboutButtonPattern.Current.IsSelected.Should().BeTrue();

    mainWindowAboutViewElement.Current.IsOffscreen.Should().BeFalse();
  }


  private static AutomationElement GetMainWindowElement()
  {
    return AutomationElement.RootElement.FindFirst(
      TreeScope.Children,
      new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationIds.MainWindow)
    );
  }
}
