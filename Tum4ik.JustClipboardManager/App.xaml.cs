using System;
using System.Windows;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  [STAThread]
  public static void Main(string[] args)
  {
    var app = new App();
    var tray = new TrayIcon();
    app.Run();
  }
}
