using System.Diagnostics.CodeAnalysis;
using Tum4ik.JustClipboardManager.Views.Main;
using Tum4ik.JustClipboardManager.Views.Main.Plugins;
using Tum4ik.JustClipboardManager.Views.Main.Settings;

namespace Tum4ik.JustClipboardManager.Views;

internal static class AppPages
{
  private static readonly Dictionary<AppPage, Type> s_pages = new() {
    { AppPage.Settings, typeof(SettingsPage) },
    { AppPage.SettingsGeneral, typeof(SettingsGeneralPage) },
    { AppPage.SettingsInterface, typeof(SettingsInterfacePage) },

    { AppPage.Plugins, typeof(PluginsPage) },
    { AppPage.About, typeof(AboutPage) },
  };

  public static bool TryGetPageType(AppPage page, [MaybeNullWhen(false)] out Type pageType)
  {
    return s_pages.TryGetValue(page, out pageType);
  }
}


internal enum AppPage
{
  Settings,
  SettingsGeneral,
  SettingsInterface,

  Plugins,
  About,
}
