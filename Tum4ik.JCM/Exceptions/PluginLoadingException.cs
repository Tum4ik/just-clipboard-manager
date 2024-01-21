namespace Tum4ik.JustClipboardManager.Exceptions;
public class PluginLoadingException : Exception
{
  public PluginLoadingException()
  {
  }

  public PluginLoadingException(string message) : base(message)
  {
  }

  public PluginLoadingException(string message, Exception innerException) : base(message, innerException)
  {
  }
}
