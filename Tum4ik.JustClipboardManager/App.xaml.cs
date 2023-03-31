using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using IWshRuntimeLibrary;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Regions.Behaviors;
using SingleInstanceCore;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Ioc;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.PInvoke;
using Tum4ik.JustClipboardManager.Services.Theme;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels;
using Tum4ik.JustClipboardManager.ViewModels.Main;
using Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
using Tum4ik.JustClipboardManager.ViewModels.Shared;
using Tum4ik.JustClipboardManager.Views;
using Tum4ik.JustClipboardManager.Views.Main;
using Tum4ik.JustClipboardManager.Views.Main.Settings;
using Tum4ik.JustClipboardManager.Views.Shared;

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

    var instanceUniqueName = "JustClipboardManager_B9D1525B-D41C-49E0-83F7-038339056F46";
#if DEBUG
    instanceUniqueName += "_Development";
#endif
    var isFirstInstance = app.InitializeAsFirstInstance(instanceUniqueName);
    if (!isFirstInstance)
    {
      return;
    }

    app.DispatcherUnhandledException += (s, e) =>
    {
      // TODO: improve to give user a chance to decide send or not
      Crashes.TrackError(e.Exception, new Dictionary<string, string>
      {
        { "Message", "Application Dispatcher Unhandled Exception" }
      });
      Task.Delay(10000).Wait(); // Give Crashes some time to be able to record exception properly
      e.Handled = true;
      app.Shutdown();
    };
    app.InitializeComponent();
    app.OverrideDefaultProperties();
    app.Run();
  }


  internal void OverrideDefaultProperties()
  {
    FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
    {
      DefaultValue = FindResource(typeof(Window))
    });
  }


  public void OnInstanceInvoked(string[] args)
  {
    // TODO: maybe show "already started" notification
  }


  private ServiceProvider? _serviceProvider;


  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    _serviceProvider = ConfigureServices();
    var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
    AppCenter.Start(configuration["MicrosoftAppCenterSecret"], typeof(Crashes), typeof(Analytics));
    ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) => _serviceProvider.GetRequiredService(type));
    ContainerLocator.SetContainerExtension(() => new ServiceContainerExtension(_serviceProvider));

    var updateService = _serviceProvider.GetRequiredService<IUpdateService>();
    updateService.SilentUpdate();

    var regionAdapterMappings = _serviceProvider.GetRequiredService<RegionAdapterMappings>();
    RegisterDefaultRegionAdapterMappings(regionAdapterMappings);

    var defaultRegionBehaviors = _serviceProvider.GetRequiredService<IRegionBehaviorFactory>();
    RegisterDefaultRegionBehaviors(defaultRegionBehaviors);

    //var moduleManager = _serviceProvider.GetRequiredService<IModuleManager>();
    //moduleManager.Run();

    using var dbContext = _serviceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext();
    dbContext.Database.Migrate();

    RemoveOldClips(_serviceProvider);
    var trayIcon = _serviceProvider.GetRequiredService<TrayIcon>();
    var hookService = _serviceProvider.GetRequiredService<GeneralHookService>();
    var clipboardService = _serviceProvider.GetRequiredService<IClipboardService>();
  }


  protected override void OnExit(ExitEventArgs e)
  {
    _serviceProvider?.Dispose();
    SingleInstance.Cleanup();

    base.OnExit(e);
  }


  private static void RemoveOldClips(ServiceProvider serviceProvider)
  {
    var clipRepository = serviceProvider.GetRequiredService<IClipRepository>();
    clipRepository.DeleteBeforeDateAsync(DateTime.Now.AddMonths(-3)).Await(e => Crashes.TrackError(e)); // TODO: before date from settings
  }


  private static ServiceProvider ConfigureServices()
  {
    var services = new ServiceCollection();

    services
      .AddSingleton<IContainerExtension>(sp => new ServiceContainerExtension(sp))
      .AddConfiguration()
      //.AddSingleton<IModuleCatalog>(new DirectoryModuleCatalog { ModulePath = "Modules" })
      .AddPrismServices()
      .AddPrismBehaviors()
      .AddRegionAdapters()
      .AddDatabase()
      .AddSingleton<IUser32DllService, User32DllService>()
      .AddSingleton<ISHCoreDllService, SHCoreDllService>()
      .AddSingleton<IKernel32DllService, Kernel32DllService>()
      .AddSingleton<GeneralHookService>()
      .AddSingleton<IKeyboardHookService, KeyboardHookService>()
      .AddSingleton<IClipboardHookService, ClipboardHookService>()
      .AddSingleton<IPasteWindowService, PasteWindowService>()
      .AddSingleton<IPasteService, PasteService>()
      .AddSingleton<IClipboardService, ClipboardService>()
      .AddSingleton<ISettingsService, SettingsService>()
      .AddSingleton<ITranslationService, TranslationService>()
      .AddSingleton<IThemeService, ThemeService>()
      .AddTransient<IKeyBindingRecordingService, KeyBindingRecordingService>()
      .AddTransient<IClipRepository, ClipRepository>()
      .AddTransient<IInfoService, InfoService>()
      .AddTransient<IUpdateService, UpdateService>()
      .AddTransient<IGitHubClient>(sp =>
      {
        var infoService = sp.GetRequiredService<IInfoService>();
        return new GitHubClient(new ProductHeaderValue("JustClipboardManager", infoService.InformationalVersion));
      })
      .AddTransient(sp => new WshShell())
      .AddTransient<IShortcutService, ShortcutService>()
      .RegisterShell<TrayIcon, TrayIconViewModel>(ServiceLifetime.Singleton)
      .RegisterShell<PasteWindow, PasteWindowViewModel>(ServiceLifetime.Singleton)
      .RegisterDialogWindow<MainDialogWindow>(WindowNames.MainAppWindow)
      .RegisterDialogWindow<SimpleDialogWindow>(WindowNames.SimpleDialogWindow)
      .RegisterSingleInstanceDialog<MainDialog, MainDialogViewModel>(DialogNames.MainDialog)
      .RegisterDialog<UnregisteredHotkeysDialog, UnregisteredHotkeysDialogViewModel>(DialogNames.UnregisteredHotkeysDialog)
      .RegisterDialog<EditHotkeyDialog, EditHotkeyDialogViewModel>(DialogNames.EditHotkeyDialog)
      .RegisterForNavigation<SettingsView, SettingsViewModel>(ViewNames.SettingsView)
      .RegisterForNavigation<SettingsGeneralView, SettingsGeneralViewModel>(ViewNames.SettingsGeneralView)
      .RegisterForNavigation<SettingsInterfaceView, SettingsInterfaceViewModel>(ViewNames.SettingsInterfaceView)
      .RegisterForNavigation<SettingsHotkeysView, SettingsHotkeysViewModel>(ViewNames.SettingsHotkeysView)
      .RegisterForNavigation<AboutView, AboutViewModel>(ViewNames.AboutView);

    return services.BuildServiceProvider();
  }


  private static void RegisterDefaultRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
  {
    regionAdapterMappings.RegisterMapping<Selector, SelectorRegionAdapter>();
    regionAdapterMappings.RegisterMapping<ItemsControl, ItemsControlRegionAdapter>();
    regionAdapterMappings.RegisterMapping<ContentControl, ContentControlRegionAdapter>();
  }


  private static void RegisterDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
  {
    regionBehaviors.AddIfMissing<BindRegionContextToDependencyObjectBehavior>(BindRegionContextToDependencyObjectBehavior.BehaviorKey);
    regionBehaviors.AddIfMissing<RegionActiveAwareBehavior>(RegionActiveAwareBehavior.BehaviorKey);
    regionBehaviors.AddIfMissing<SyncRegionContextWithHostBehavior>(SyncRegionContextWithHostBehavior.BehaviorKey);
    regionBehaviors.AddIfMissing<RegionManagerRegistrationBehavior>(RegionManagerRegistrationBehavior.BehaviorKey);
    regionBehaviors.AddIfMissing<RegionMemberLifetimeBehavior>(RegionMemberLifetimeBehavior.BehaviorKey);
    regionBehaviors.AddIfMissing<ClearChildViewsRegionBehavior>(ClearChildViewsRegionBehavior.BehaviorKey);
    regionBehaviors.AddIfMissing<AutoPopulateRegionBehavior>(AutoPopulateRegionBehavior.BehaviorKey);
    regionBehaviors.AddIfMissing<DestructibleRegionBehavior>(DestructibleRegionBehavior.BehaviorKey);
  }
}
