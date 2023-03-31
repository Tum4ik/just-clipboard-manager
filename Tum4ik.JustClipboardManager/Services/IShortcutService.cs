namespace Tum4ik.JustClipboardManager.Services;

internal interface IShortcutService
{
  /// <summary>
  /// Checks the application shortcut exists in the special folder.
  /// </summary>
  /// <param name="specialFolder"></param>
  /// <returns></returns>
  bool Exists(Environment.SpecialFolder specialFolder, out string path);

  /// <summary>
  /// Creates the application shortcut in the special folder if it does not exist.
  /// </summary>
  /// <param name="specialFolder"></param>
  void Create(Environment.SpecialFolder specialFolder);

  /// <summary>
  /// Deletes the application shortcut in the special folder if it exists.
  /// </summary>
  /// <param name="specialFolder"></param>
  void Delete(Environment.SpecialFolder specialFolder);
}
