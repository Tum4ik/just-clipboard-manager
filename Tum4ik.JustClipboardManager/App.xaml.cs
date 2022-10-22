using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SingleInstanceCore;
using Tum4ik.EventAggregator.Extensions;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.ViewModels;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : ISingleInstance
{
  [STAThread]
  public static void Main(string[] args)
  {
    var app = new App();

    var isFirstInstance = app.InitializeAsFirstInstance("JustClipboardManager_B9D1525B-D41C-49E0-83F7-038339056F46");
    if (!isFirstInstance)
    {
      return;
    }

    app.DispatcherUnhandledException += (s, e) =>
    {
      Crashes.TrackError(e.Exception); // TODO: improve to give user a chance to decide send or not
      Task.Delay(10000).Wait(); // Give Crashes some time to be able to record exception properly
      e.Handled = true;
      app.Shutdown();
    };
    app.InitializeComponent();
    app.Run();
  }


  public void OnInstanceInvoked(string[] args)
  {
    // TODO: maybe show "already started" notification
  }


  private IConfiguration? _configuration;
  public IConfiguration Configuration => _configuration ??= ConfigureAppConfiguration();

  private ServiceProvider? _serviceProvider;
  public IServiceProvider ServiceProvider => _serviceProvider ??= ConfigureServices(Configuration);


  protected override void OnStartup(StartupEventArgs e)
  {
    AppCenter.Start(Configuration["MicrosoftAppCenterSecret"], typeof(Crashes), typeof(Analytics));

    base.OnStartup(e);

    RemoveOldClips();
    var trayIcon = ServiceProvider.GetRequiredService<TrayIcon>();
    var hookService = ServiceProvider.GetRequiredService<GeneralHookService>();
    var clipboardService = ServiceProvider.GetRequiredService<IClipboardService>();
  }


  protected override void OnExit(ExitEventArgs e)
  {
    _serviceProvider?.Dispose();
    SingleInstance.Cleanup();

    base.OnExit(e);
  }


  private void RemoveOldClips()
  {
    var clipRepository = ServiceProvider.GetRequiredService<IClipRepository>();
    clipRepository.DeleteBeforeDateAsync(DateTime.Now.AddMonths(-3)); // TODO: befor date from settings
  }


  private static IConfiguration ConfigureAppConfiguration()
  {
    var assembly = Assembly.GetExecutingAssembly();
    var appsettingsResourceName = assembly.GetManifestResourceNames()
      .Single(n => n.EndsWith("appsettings.json", StringComparison.InvariantCultureIgnoreCase));
    // Don't use "using" keyword for appsettingsStream here - it will break the settings reading process.
    // The stream will be disposed by StreamReader internally anyway.
    var appsettingsStream = assembly.GetManifestResourceStream(appsettingsResourceName);
    return new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonStream(appsettingsStream)
#if DEBUG
      .AddUserSecrets(Assembly.GetExecutingAssembly())
#endif
      .Build();
  }


  private static ServiceProvider ConfigureServices(IConfiguration configuration)
  {
    var services = new ServiceCollection();

    services
      .AddSingleton(configuration)
      .AddPooledDbContextFactory<AppDbContext>((sp, o) =>
      {
        var dbFileDir = Path.GetDirectoryName(
          System.Configuration.ConfigurationManager
            .OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal)
            .FilePath
        )!;
        Directory.CreateDirectory(dbFileDir);
        var dbFilePath = Path.Combine(dbFileDir, "clips.db");
        o.UseSqlite($"Data Source={dbFilePath}")
         .UseLazyLoadingProxies();
      }, 2)
      .AddTransient<IClipRepository, ClipRepository>()
      .AddEventAggregator()
      .AddSingleton<GeneralHookService>()
      .AddSingleton<IKeyboardHookService, KeyboardHookService>()
      .AddSingleton<IClipboardHookService, ClipboardHookService>()
      .AddSingleton<IPasteWindowService, PasteWindowService>()
      .AddSingleton<IPasteService, PasteService>()
      .AddSingleton<IClipboard, ClipboardWrapper>()
      .AddSingleton<IClipboardService, ClipboardService>()
      .RegisterView<TrayIcon, TrayIconViewModel>(ServiceLifetime.Singleton)
      .RegisterView<PasteWindow, PasteWindowViewModel>(ServiceLifetime.Singleton);

    return services.BuildServiceProvider();
  }
}
