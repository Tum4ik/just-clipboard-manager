namespace Tum4ik.JustClipboardManager.PluginDevKit.Exceptions;
public class PluginInitializationException : Exception
{
  public PluginInitializationException()
  {
  }

  public PluginInitializationException(string message) : base(message)
  {
  }

  public PluginInitializationException(string message, Exception innerException) : base(message, innerException)
  {
  }
}
