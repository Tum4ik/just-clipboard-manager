using System.Windows.Input;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.Services;
internal class KeyBindingRecordingService : IKeyBindingRecordingService
{
  private ModifierKeys _pressedModifiers;
  private Key _pressedKey;


  public (KeyBindingDescriptor descriptor, bool completed) RecordKeyDown(Key key)
  {
    return RecordKey(key, AddModifierKey, k =>
    {
      if (_pressedKey == Key.None)
      {
        _pressedKey = k;
      }
    });
  }


  public (KeyBindingDescriptor descriptor, bool completed) RecordKeyUp(Key key)
  {
    return RecordKey(key, RemoveModifierKey, k =>
    {
      if (_pressedKey == k)
      {
        _pressedKey = Key.None;
      }
    });
  }


  public (KeyBindingDescriptor descriptor, bool completed) ResetRecord()
  {
    _pressedModifiers = ModifierKeys.None;
    _pressedKey = Key.None;
    return (new(_pressedModifiers, _pressedKey), false);
  }


  public bool Completed => _pressedModifiers != ModifierKeys.None && _pressedKey != Key.None;


  private void AddModifierKey(ModifierKeys modifier)
  {
    if (!_pressedModifiers.HasFlag(modifier))
    {
      _pressedModifiers |= modifier;
    }
  }


  private void RemoveModifierKey(ModifierKeys modifier)
  {
    if (_pressedModifiers.HasFlag(modifier))
    {
      _pressedModifiers &= ~modifier;
    }
  }


  private (KeyBindingDescriptor descriptor, bool completed) RecordKey(Key key,
                                                                      Action<ModifierKeys> modifierAction,
                                                                      Action<Key> keyAction)
  {
    if (!Completed)
    {
      switch (key)
      {
        case Key.LeftAlt:
        case Key.RightAlt:
          modifierAction(ModifierKeys.Alt);
          break;
        case Key.LeftCtrl:
        case Key.RightCtrl:
          modifierAction(ModifierKeys.Control);
          break;
        case Key.LeftShift:
        case Key.RightShift:
          modifierAction(ModifierKeys.Shift);
          break;
        case Key.LWin:
        case Key.RWin:
          modifierAction(ModifierKeys.Windows);
          break;
        default:
          keyAction(key);
          break;
      }
    }

    return (new(_pressedModifiers, _pressedKey), Completed);
  }
}
