using System;
using JustClipboardManager.Application;
using JustClipboardManager.Application.Services;
using JustClipboardManager.Infrastructure;
using JustClipboardManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;

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


    private static ServiceProvider ConfigureServices()
    {
      var services = new ServiceCollection();

      services.AddApplication();
      services.AddInfrastructure();
      services.AddTransient<MainWindow>();
      services.AddSingleton<ITrayIconService, TrayIconService>();
      services.AddSingleton<IWindowingService, WindowingService>();
      services.AddSingleton<IClipboardService, ClipboardService>();
      services.AddSingleton<IPluginsService<DataPackageView, DataPackage>, PluginsService>();

      return services.BuildServiceProvider();
    }


    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
      // todo: show migration progress, maybe provide cancellation possibility
      await _services.GetRequiredService<IDatabaseMigrator>().MigrateAsync();

      _services.GetRequiredService<ITrayIconService>().Initialize();
      _services.GetRequiredService<IClipboardService>().Initialize();
      _services.GetRequiredService<IPluginsService<DataPackageView, DataPackage>>().LoadPlugins();
    }
  }
}
