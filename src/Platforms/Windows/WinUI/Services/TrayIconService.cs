using System;
using System.Drawing;
using System.IO;
using H.NotifyIcon.Core;
using JustClipboardManager.Application.Services;

namespace JustClipboardManager.Services;

internal class TrayIconService : ITrayIconService
{
  private readonly Stream _iconStream;
  private readonly Icon _icon;
  private readonly TrayIconWithContextMenu _trayIcon;

  public TrayIconService()
  {
    _iconStream = H.Resources.icon_ico.AsStream();
    _icon = new Icon(_iconStream);
    _trayIcon = new TrayIconWithContextMenu
    {
      Icon = _icon.Handle,
      ToolTip = "Just Clipboard Manager",
      ContextMenu = new PopupMenu
      {
        Items =
        {
          new PopupMenuItem("Settings", (_, _) => {}),
          new PopupMenuItem("About", (_, _) => {}),
          new PopupMenuSeparator(),
          new PopupSubMenu("Language")
          {
            Items =
            {
              new PopupMenuItem("English", (_, _) => {}),
              new PopupMenuItem("Ukraine", (_, _) => {}),
            }
          },
          new PopupMenuSeparator(),
          new PopupMenuItem("Exit", (_, _) => Exit()),
        },
      }
    };
  }

  public void Initialize()
  {
    _trayIcon.Create();
  }


  private void Exit()
  {
    _trayIcon.Dispose();
    _icon.Dispose();
    _iconStream.Dispose();
    Environment.Exit(0);
  }
}
