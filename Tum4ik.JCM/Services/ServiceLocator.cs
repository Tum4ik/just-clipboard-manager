using DryIoc;
using Tum4ik.JustClipboardManager.Exceptions;

namespace Tum4ik.JustClipboardManager.Services;

internal static class ServiceLocator
{
  private static IResolver? s_current;

  public static IResolver Current => s_current
    ?? throw new ServiceLocatorException("ServiceLocator is not initialized.");

  public static void Initialize(IResolver resolver)
  {
    if (s_current is not null)
    {
      throw new ServiceLocatorException("ServiceLocator is already initialized.");
    }
    s_current = resolver;
  }
}
