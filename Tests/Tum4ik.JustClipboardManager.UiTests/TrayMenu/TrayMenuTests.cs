using Tum4ik.JustClipboardManager.UiTests.Helpers;

namespace Tum4ik.JustClipboardManager.UiTests.TrayMenu;

[Collection(Collections.Application)]
public class TrayMenuTests
{
  [Fact]
  internal void SettingsClickTest()
  {
    AppActionsHelper.TrayIconRightClick();
  }
}
