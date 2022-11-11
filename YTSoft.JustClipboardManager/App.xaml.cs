using System.Reflection;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace YTSoft.JustClipboardManager;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App
{
  [STAThread]
  public static async Task Main(string[] args)
  {
    if (IsRedirect(out var instance))
    {
      var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
      await instance.RedirectActivationToAsync(activatedArgs);
      return;
    }

    Start(p =>
    {
      var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
      SynchronizationContext.SetSynchronizationContext(context);
      var app = new App();
      app.UnhandledException += (s, e) =>
      {
        Crashes.TrackError(e.Exception); // TODO: improve to give user a chance to decide send or not
        Task.Delay(10000).Wait(); // Give Crashes some time to be able to record exception properly
        e.Handled = true;
        app.Exit();
      };
    });
  }


  private static bool IsRedirect(out AppInstance instance)
  {
    instance = AppInstance.FindOrRegisterForKey("JustClipboardManager_B9D1525B-D41C-49E0-83F7-038339056F46");
    return !instance.IsCurrent;
  }


  private readonly IHost _host;

  public App()
  {
    InitializeComponent();
    _host = Host.CreateDefaultBuilder()
      .ConfigureAppConfiguration((context, builder) =>
      {
        builder.Properties.Clear();
        builder.Sources.Clear();
        var assembly = Assembly.GetExecutingAssembly();
        var appsettingsResourceName = assembly
          .GetManifestResourceNames()
          .Single(n => n.EndsWith("appsettings.json", StringComparison.InvariantCultureIgnoreCase));
        // Don't use "using" keyword for appsettingsStream here - it will break the settings reading process.
        // The stream will be disposed by StreamReader internally anyway.
        var appsettingsStream = assembly.GetManifestResourceStream(appsettingsResourceName)!;
        builder.AddJsonStream(appsettingsStream);
#if DEBUG
        builder.AddUserSecrets(Assembly.GetExecutingAssembly());
#endif
      })
      .ConfigureServices((context, services) =>
      {
        var msappc = context.Configuration["MicrosoftAppCenterSecret"];
      })
      .Build();
  }


  protected override void OnLaunched(LaunchActivatedEventArgs args)
  {
    base.OnLaunched(args);
  }
}
