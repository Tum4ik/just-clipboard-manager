using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SingleInstanceCore;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.ViewModels;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, ISingleInstance
{
  [STAThread]
  public static void Main(string[] args)
  {
    var app = new App
    {
      ShutdownMode = ShutdownMode.OnExplicitShutdown
    };

    var isFirstInstance = app.InitializeAsFirstInstance("JustClipboardManager_B9D1525B-D41C-49E0-83F7-038339056F46");
    if (!isFirstInstance)
    {
      return;
    }

    app.DispatcherUnhandledException += (s, e) =>
    {
      Crashes.TrackError(e.Exception); // TODO: improve to give user a chance to decide send or not
      e.Handled = true;
      app.Shutdown();
    };
    app.Run();
  }


  public void OnInstanceInvoked(string[] args)
  {
    // TODO: maybe show "already started" notification
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
    AppCenter.Start(Configuration["MicrosoftAppCenterSecret"], typeof(Crashes));

    base.OnStartup(e);

    var trayIcon = ServiceProvider.GetRequiredService<TrayIcon>();
  }


  protected override void OnExit(ExitEventArgs e)
  {
    var keyboardHookService = ServiceProvider.GetRequiredService<IKeyboardHookService>();
    keyboardHookService.Stop();
    SingleInstance.Cleanup();

    base.OnExit(e);
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
      .AddJsonFile("appsettings.Development.json", true)
      .Build();
  }


  private IServiceProvider ConfigureServices(IConfiguration configuration)
  {
    var services = new ServiceCollection();

    services
      .AddSingleton(configuration)
      .AddSingleton<IKeyboardHookService, KeyboardHookService>()
      .AddSingleton<IPasteWindowService, PasteWindowService>()
      .AddSingleton<IPasteService, PasteService>()
      .RegisterView<TrayIcon, TrayIconViewModel>(ServiceLifetime.Singleton)
      .RegisterView<PasteWindow, PasteWindowViewModel>(ServiceLifetime.Singleton);

    return services.BuildServiceProvider();
  }
}
