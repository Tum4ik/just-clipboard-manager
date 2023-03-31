namespace Tum4ik.JustClipboardManager.Ioc;
internal static class ServiceKeyToTypeMappings
{
  private static readonly Dictionary<string, Type> s_mappings = new();


  public static void Add(string key, Type type)
  {
    s_mappings.Add(key, type);
  }


  public static Type? Get(string key)
  {
    if (s_mappings.TryGetValue(key, out var type))
    {
      return type;
    }

    return null;
  }
}
