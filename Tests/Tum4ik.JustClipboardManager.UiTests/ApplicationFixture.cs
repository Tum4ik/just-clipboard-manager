using System.Diagnostics;

namespace Tum4ik.JustClipboardManager.UiTests;

public sealed class ApplicationFixture : IDisposable
{
  private readonly Process _appProcess;

  public ApplicationFixture()
  {
    const string AppExePath = @"..\..\..\..\..\Tum4ik.JustClipboardManager\bin\x64\Debug\net8.0-windows\JustClipboardManager.exe";
    _appProcess = Process.Start(AppExePath, "--uitest");
  }


  public void Dispose()
  {
    _appProcess.Kill();
    _appProcess.Dispose();
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
