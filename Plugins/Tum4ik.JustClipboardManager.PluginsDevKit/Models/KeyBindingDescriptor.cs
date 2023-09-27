using System.Windows.Input;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Models;

public record KeyBindingDescriptor(ModifierKeys Modifiers, Key Key)
{
  public override string ToString()
  {
    var modifiers = Modifiers.ToString()
      .Replace(",", " +", StringComparison.OrdinalIgnoreCase)
      .Replace(ModifierKeys.None.ToString(), string.Empty, StringComparison.OrdinalIgnoreCase)
      .Replace(ModifierKeys.Control.ToString(), "Ctrl", StringComparison.OrdinalIgnoreCase)
      .Replace(ModifierKeys.Windows.ToString(), "Win", StringComparison.OrdinalIgnoreCase);
    var key = Key.ToString()
      .Replace(Key.None.ToString(), string.Empty, StringComparison.OrdinalIgnoreCase)
      .Replace(Key.Escape.ToString(), "Esc", StringComparison.OrdinalIgnoreCase);
    var separator = string.IsNullOrEmpty(modifiers) || string.IsNullOrEmpty(key) ? string.Empty : " + ";
    return $"{modifiers}{separator}{key}";
  }
}
