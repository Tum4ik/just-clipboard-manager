namespace Tum4ik.JustClipboardManager.Exceptions;

public class ServiceLocatorException : Exception
{
  public ServiceLocatorException()
  {
  }

  public ServiceLocatorException(string? message) : base(message)
  {
  }

  public ServiceLocatorException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
