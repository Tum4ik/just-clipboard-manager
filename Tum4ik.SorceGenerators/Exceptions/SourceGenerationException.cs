using System;
using System.Runtime.Serialization;

namespace Tum4ik.SorceGenerators.Exceptions
{
  [Serializable]
  public class SourceGenerationException : Exception
  {
    public SourceGenerationException()
    {
    }

    public SourceGenerationException(string message) : base(message)
    {
    }

    public SourceGenerationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SourceGenerationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
