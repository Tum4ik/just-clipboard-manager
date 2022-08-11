namespace Tum4ik.Attributes;

/// <summary>
/// An attribute that can be used to automatically generate interface members and an implementation wrapper for a
/// static class.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public class IIForStaticClassAttribute : Attribute
{
  public IIForStaticClassAttribute(Type staticClassType, string implementationClassName)
  {
    StaticClassType = staticClassType;
    ImplementationClassName = implementationClassName;
  }


  public Type StaticClassType { get; }
  public string ImplementationClassName { get; }
}
