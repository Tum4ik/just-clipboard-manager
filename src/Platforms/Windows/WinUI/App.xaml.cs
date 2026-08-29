using System;
using JustClipboardManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using JustClipboardManager.Application.Services;
using JustClipboardManager.Infrastructure;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JustClipboardManager
{
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  public partial class App : Microsoft.UI.Xaml.Application
  {
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
      _services = ConfigureServices();
      InitializeComponent();
    }


    private static IServiceProvider ConfigureServices()
    {
      var services = new ServiceCollection();

      services.AddInfrastructure();
      services.AddSingleton<ITrayIconService, TrayIconService>();
      services.AddSingleton<IWindowingService, WindowingService>();

      return services.BuildServiceProvider();
    }


    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
      _services.GetRequiredService<ITrayIconService>().Initialize();
    }
  }
}
