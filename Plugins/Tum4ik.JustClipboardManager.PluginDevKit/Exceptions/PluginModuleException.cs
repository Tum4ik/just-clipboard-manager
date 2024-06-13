namespace Tum4ik.JustClipboardManager.PluginDevKit.Exceptions;
public class PluginModuleException : Exception
{
  public PluginModuleException()
  {
  }

  public PluginModuleException(string message) : base(message)
  {
  }

  public PluginModuleException(string message, Exception innerException) : base(message, innerException)
  {
  }
}
