using System.Windows;

namespace Tum4ik.JustClipboardManager.UnitTests;
internal sealed class ApplicationInstanceRequiredFixture : IDisposable
{
  private static readonly object s_locker = new();
  private readonly Application? _application;

  public ApplicationInstanceRequiredFixture()
  {
    // To be able to run the test classes with this fixture individually, we have to apply lock here.
    lock (s_locker)
    {
      if (Application.Current is null)
      {
        _application = new Application();
      }
    }
  }


  public void Dispose()
  {
    _application?.Shutdown();
  }
}


[CollectionDefinition(TestCollections.ApplicationInstanceRequired, DisableParallelization = true)]
public class ApplicationInstanceRequiredCollection : ICollectionFixture<ApplicationInstanceRequiredFixture>
{
}
