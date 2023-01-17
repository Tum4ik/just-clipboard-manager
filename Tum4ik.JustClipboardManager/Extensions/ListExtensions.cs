using System.Diagnostics.CodeAnalysis;

namespace Tum4ik.JustClipboardManager.Extensions;

internal static class ListExtensions
{
  public static bool TryGet<T>(this IList<T> list, int index, [NotNullWhen(true)] out T? item)
  {
    if (index < 0 || index >= list.Count)
    {
      item = default;
      return false;
    }

    item = list[index]!;
    return true;
  }
}
