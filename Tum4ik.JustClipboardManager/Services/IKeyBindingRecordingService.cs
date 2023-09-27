using System.Windows.Input;
using Tum4ik.JustClipboardManager.PluginDevKit.Models;

namespace Tum4ik.JustClipboardManager.Services;

/// <summary>
/// Provides functionality to record key binding (hotkey).
/// </summary>
internal interface IKeyBindingRecordingService
{
  /// <summary>
  /// Adds pressed key to the <see cref="KeyBindingDescriptor"/>
  /// if the <see cref="KeyBindingDescriptor"/> is not completed yet.
  /// </summary>
  /// <param name="key">The pressed key.</param>
  /// <returns>The <see cref="KeyBindingDescriptor"/> and its completion state.</returns>
  (KeyBindingDescriptor descriptor, bool completed) RecordKeyDown(Key key);

  /// <summary>
  /// Removes released key from the <see cref="KeyBindingDescriptor"/>
  /// if the <see cref="KeyBindingDescriptor"/> is not completed yet.
  /// </summary>
  /// <param name="key">The released key.</param>
  /// <returns>The <see cref="KeyBindingDescriptor"/> and its completion state.</returns>
  (KeyBindingDescriptor descriptor, bool completed) RecordKeyUp(Key key);

  /// <summary>
  /// Resets the <see cref="KeyBindingDescriptor"/> if the <see cref="KeyBindingDescriptor"/> is already completed.
  /// </summary>
  /// <returns>
  /// The <see cref="KeyBindingDescriptor"/> and its completion state.
  /// The completion state is always false here.
  /// </returns>
  (KeyBindingDescriptor descriptor, bool completed) ResetRecord();

  bool Completed { get; }
}
