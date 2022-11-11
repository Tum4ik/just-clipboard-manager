using Microsoft.AppCenter.Crashes;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace YTSoft.JustClipboardManager;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
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


  public App()
  {
    InitializeComponent();
  }


  protected override void OnLaunched(LaunchActivatedEventArgs args)
  {
    m_window = new MainWindow();
    m_window.Activate();
  }

  private Window m_window;
}
