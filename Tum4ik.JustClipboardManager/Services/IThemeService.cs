namespace Tum4ik.JustClipboardManager.Services;

internal interface IThemeService
{
  Theme CurrentTheme { get; }
  void SetTheme(Theme theme);
}


internal enum Theme
{
  Light, Dark
}
