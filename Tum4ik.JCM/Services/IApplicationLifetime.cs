namespace Tum4ik.JustClipboardManager.Services;

/// <summary>
/// Provides the functionality to control the application lifetime.
/// </summary>
internal interface IApplicationLifetime
{
  /// <summary>
  /// Shuts down the application.
  /// </summary>
  void ExitApplication();
}
