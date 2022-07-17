using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.ViewModels;
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
    app.Run();
  }


  public IConfiguration Configuration { get; }
  public IServiceProvider ServiceProvider { get; }


  public App()
  {
    Configuration = ConfigureAppConfiguration();
    ServiceProvider = ConfigureServices(Configuration);
  }


  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);
    var trayIcon = ServiceProvider.GetRequiredService<TrayIcon>();
  }


  private IConfiguration ConfigureAppConfiguration()
  {
    var assembly = Assembly.GetExecutingAssembly();
    var appsettingsResourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("appsettings.json"));
    // Don't use "using" keyword for appsettingsStream here - it will break the settings reading process.
    // The stream will be disposed by StreamReader internally anyway.
    var appsettingsStream = assembly.GetManifestResourceStream(appsettingsResourceName);
    return new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonStream(appsettingsStream)
      .Build();
  }


  private IServiceProvider ConfigureServices(IConfiguration configuration)
  {
    var services = new ServiceCollection();

    services
      .AddSingleton(configuration)
      .RegisterView<TrayIcon, TrayIconViewModel>();

    return services.BuildServiceProvider();
  }
}
