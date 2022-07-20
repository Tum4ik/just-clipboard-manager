using System;
using System.Runtime.Serialization;

namespace Tum4ik.JustClipboardManager.Exceptions;

[Serializable]
public class HotKeyRegistrationException : Exception
{
  public HotKeyRegistrationException()
  {
  }

  public HotKeyRegistrationException(string? message) : base(message)
  {
  }

  public HotKeyRegistrationException(string? message, Exception? innerException) : base(message, innerException)
  {
  }

  protected HotKeyRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }
}
