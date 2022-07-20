using System;
using System.Runtime.Serialization;

namespace Tum4ik.JustClipboardManager.Exceptions;

[Serializable]
public class ServiceInitializationException : Exception
{
  public ServiceInitializationException()
  {
  }

  public ServiceInitializationException(string? message) : base(message)
  {
  }

  public ServiceInitializationException(string? message, Exception? innerException) : base(message, innerException)
  {
  }

  protected ServiceInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }
}
