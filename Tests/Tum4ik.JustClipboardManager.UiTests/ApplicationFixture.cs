using System.Diagnostics;
using System.IO;
using System.Windows.Automation;
using Tum4ik.JustClipboardManager.UiTests.Exceptions;
using Tum4ik.JustClipboardManager.UiTests.Extensions;

namespace Tum4ik.JustClipboardManager.UiTests;

public sealed class ApplicationFixture : IDisposable
{
  private readonly Process _appProcess;

  public ApplicationFixture()
  {
    string[] appExePossiblePlaces = [
      @"..\..\..\..\..\Tum4ik.JustClipboardManager\bin\x64\Debug\net8.0-windows\JustClipboardManager.exe",
      @"..\..\..\..\..\Tum4ik.JustClipboardManager\bin\x64\Release\net8.0-windows\JustClipboardManager.exe",
      @"..\..\..\..\..\Tum4ik.JustClipboardManager\bin\publish\JustClipboardManager.exe",
    ];
    foreach (var appExePath in appExePossiblePlaces)
    {
      if (File.Exists(appExePath))
      {
        _appProcess = Process.Start(appExePath, "--uitest");
        TrayIconElement = GetTrayIconElement();
        return;
      }
    }
    throw new FileNotFoundException("Application exe file is not found.");
  }


  public void Dispose()
  {
    _appProcess.Kill();
    _appProcess.Dispose();
  }


  public AutomationElement TrayIconElement { get; }


  private static AutomationElement GetTrayIconElement()
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
        return icon;
      }
    }
    throw new ElementNotFoundException("Tray icon is not found");
  }
}


public static class Collections
{
  public const string Application = "Application Collection";
}


[CollectionDefinition(Collections.Application)]
public class ApplicationCollectionFixture : ICollectionFixture<ApplicationFixture>
{
}
