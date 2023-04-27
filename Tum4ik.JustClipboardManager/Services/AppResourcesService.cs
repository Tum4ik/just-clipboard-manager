using System.Windows;

namespace Tum4ik.JustClipboardManager.Services;
internal class AppResourcesService : IAppResourcesService
{
  public AppResourcesService(ResourceDictionary resources)
  {
    Resources = resources;
  }

  public ResourceDictionary Resources { get; }
}
