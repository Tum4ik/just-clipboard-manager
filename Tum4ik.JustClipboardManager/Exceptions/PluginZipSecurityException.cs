namespace Tum4ik.JustClipboardManager.Exceptions;
public class PluginZipSecurityException : Exception
{
  public PluginZipSecurityException(string message) : base(message)
  {
  }

  public PluginZipSecurityException(string message, Exception innerException) : base(message, innerException)
  {
  }

  public PluginZipSecurityException()
  {
  }
}
