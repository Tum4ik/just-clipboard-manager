using System.Windows;
using System.Windows.Threading;
using DeviceId;
using DryIoc;
using IWshRuntimeLibrary;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Threading;
using Octokit;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Services.Dialogs;
using Sentry.Extensibility;
using SingleInstanceCore;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Helpers;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Properties;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using Tum4ik.JustClipboardManager.Services.Plugins;
using Tum4ik.JustClipboardManager.Services.Theme;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels;
using Tum4ik.JustClipboardManager.ViewModels.Main;
using Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;
using Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
using Tum4ik.JustClipboardManager.ViewModels.Shared;
using Tum4ik.JustClipboardManager.Views;
using Tum4ik.JustClipboardManager.Views.Main;
using Tum4ik.JustClipboardManager.Views.Main.Plugins;
using Tum4ik.JustClipboardManager.Views.Main.Settings;
using Tum4ik.JustClipboardManager.Views.Shared;

namespace Tum4ik.JustClipboardManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : ISingleInstance, IApplicationLifetime
{
  private const string RestartAfterCrashArg = "--restart-after-crash";
  private const string RestartAfterCrashDelimiter = ":";
  private static int RestartAfterCrashCount;


  [STAThread]
  public static void Main(string[] args)
  {
    var app = new App(args);
    
    var instanceUniqueName = "JustClipboardManager_B9D1525B-D41C-49E0-83F7-038339056F46";
    instanceUniqueName += app._appEnvironmentService.Environment switch
    {
      AppEnvironment.Development => "_Development",
      AppEnvironment.UiTest => "_UiTest",
      _ => string.Empty,
    };
    
    var restartAfterCrashArg = Array.Find(args, a => a.StartsWith(RestartAfterCrashArg, StringComparison.Ordinal));
    var isFirstInstance = app.InitializeAsFirstInstance(instanceUniqueName);
    if (restartAfterCrashArg is null && !isFirstInstance)
    {
      return;
    }
    if (restartAfterCrashArg is not null
        && int.TryParse(restartAfterCrashArg.Split(RestartAfterCrashDelimiter)[1], out var count))
    {
      RestartAfterCrashCount = count;
    }

    if (app._appEnvironmentService.Environment == AppEnvironment.Production)
    {
      app.DispatcherUnhandledException += OnUnhandledException;
    }

    app.InitializeComponent();
    app.OverrideDefaultProperties();
    app.Run();
  }


  private readonly IConfiguration _configuration;
  private readonly IAppEnvironmentService _appEnvironmentService;


  public App(string[] args)
  {
    UpgradeSettings();
    
    AppEnvironment appEnvironment;
    (_configuration, appEnvironment) = ConfigurationHelper.CreateConfiguration(args);
    _appEnvironmentService = new AppEnvironmentService(appEnvironment);

    var deviceId = InternalSettings.Default.DeviceId;
    if (string.IsNullOrEmpty(deviceId))
    {
      deviceId = new DeviceIdBuilder()
        .OnWindows(
          w => w
            .AddWindowsDeviceId()
            .AddWindowsProductId()
            .AddMachineGuid()
        )
        .ToString();
      InternalSettings.Default.DeviceId = deviceId;
      InternalSettings.Default.Save();
    }
    SentrySdk.Init(o =>
    {
      o.Dsn = _configuration["SentryTracking:Dsn"];
      o.AutoSessionTracking = true;
      o.IsGlobalModeEnabled = true;
      o.Environment = _configuration["SentryTracking:Environment"];
    });
    SentrySdk.ConfigureScope(scope =>
    {
      scope.User = new()
      {
        Id = deviceId,
      };
    });
  }


  private static void OnUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
  {
    SentrySdk.CaptureException(e.Exception, scope =>
      scope.AddBreadcrumb("Unhandled Exception", "info", "info", dataPair: null)
    );
    e.Handled = true;
    RestartApp();
    Current.Shutdown();
  }


  private static void UpgradeSettings()
  {
    if (InternalSettings.Default.IsSettingsUpgradeRequired)
    {
      InternalSettings.Default.Upgrade();
      SettingsGeneral.Default.Upgrade();
      SettingsHotkeys.Default.Upgrade();
      SettingsInterface.Default.Upgrade();
      SettingsPasteWindow.Default.Upgrade();

      InternalSettings.Default.IsSettingsUpgradeRequired = false;
      InternalSettings.Default.Save();
    }
  }


  private static void RestartApp()
  {
    var processPath = Environment.ProcessPath;
    if (processPath is not null && RestartAfterCrashCount < 5)
    {
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(processPath)
      {
        Arguments = $"{RestartAfterCrashArg}{RestartAfterCrashDelimiter}{RestartAfterCrashCount + 1}",
        UseShellExecute = true
      });
    }
    else
    {
      // TODO: notify user about the problem, log the impossibility to restart
    }
  }


  private void OverrideDefaultProperties()
  {
    FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
    {
      DefaultValue = FindResource(typeof(Window))
    });
  }


  public async void OnInstanceInvoked(string[] args)
  {
    // TODO: maybe show "already started" notification

    if (args.Contains("--shutdown"))
    {
      var joinableTaskFactory = Container.Resolve<JoinableTaskFactory>();
      await joinableTaskFactory.SwitchToMainThreadAsync();
      Shutdown();
    }
  }


  public void ExitApplication()
  {
    Shutdown();
  }


  protected override async void OnStartup(StartupEventArgs e)
  {
    try
    {
      base.OnStartup(e);
    }
    catch (ModuleInitializeException ex)
    {
      SentrySdk.CaptureException(ex);
    }

    var updateService = Container.Resolve<IUpdateService>();
    var result = await updateService.SilentUpdateAsync().ConfigureAwait(true);
    if (result == UpdateResult.Started)
    {
      return;
    }

    var trayIcon = Container.Resolve<TrayIcon>();
    trayIcon.ForceCreate();
    

    using (var dbContext = Container.Resolve<IDbContextFactory<AppDbContext>>().CreateDbContext())
    {
      try
      {
        await dbContext.Database.MigrateAsync().ConfigureAwait(false);
      }
      catch (SqliteException)
      {
        await dbContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
        await dbContext.Database.MigrateAsync().ConfigureAwait(false);
      }
    }
    
    var settingsService = Container.Resolve<ISettingsService>();
    var clipRepository = Container.Resolve<IClipRepository>();
    var joinableTaskFactory = Container.Resolve<JoinableTaskFactory>();
    var pluginsService = Container.Resolve<IPluginsService>();

    await pluginsService.InitializeAsync().ConfigureAwait(false);
    await RemoveOldClipsAsync(settingsService, clipRepository).ConfigureAwait(false);

    await joinableTaskFactory.SwitchToMainThreadAsync();
    var hookService = Container.Resolve<GeneralHookService>();
  }


  protected override void OnExit(ExitEventArgs e)
  {
    Container.GetContainer().Dispose();
    SingleInstance.Cleanup();

    base.OnExit(e);
  }


  private static async Task RemoveOldClipsAsync(ISettingsService settingsService, IClipRepository clipRepository)
  {
    var period = settingsService.RemoveClipsPeriod;
    var periodType = settingsService.RemoveClipsPeriodType;
    var beforeDate = periodType switch
    {
      PeriodType.Day => DateTime.Now.AddDays(-period),
      PeriodType.Month => DateTime.Now.AddMonths(-period),
      PeriodType.Year => DateTime.Now.AddYears(-period),
      _ => DateTime.Now.AddMonths(-period)
    };
    await clipRepository.DeleteBeforeDateAsync(beforeDate).ConfigureAwait(false);
  }


  protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
  {
    containerRegistry.RegisterSingleton<RegionAdapterMappings>();
    containerRegistry.RegisterSingleton<IRegionManager, RegionManager>();
    containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
    containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
    containerRegistry.RegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
    containerRegistry.RegisterSingleton<IRegionBehaviorFactory, RegionBehaviorFactory>();
    containerRegistry.Register<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>();
    containerRegistry.Register<IRegionNavigationJournal, RegionNavigationJournal>();
    containerRegistry.Register<IRegionNavigationService, RegionNavigationService>();
    containerRegistry.Register<IDialogWindow, DialogWindow>(); //default dialog host
  }


  protected override void RegisterTypes(IContainerRegistry containerRegistry)
  {
    containerRegistry
      .RegisterGeneratedWrappers()
      .RegisterDatabase()
      .RegisterThreadSwitching()
      .RegisterInstance(_appEnvironmentService)
      .RegisterInstance(_configuration)
      .RegisterInstance<IApplicationLifetime>(this)
      .RegisterSingleton<IHub>(() => HubAdapter.Instance)
      .RegisterSingleton<IDialogService, ExtendedDialogService>()
      .RegisterSingleton<IUser32DllService, User32DllService>()
      .RegisterSingleton<ISHCoreDllService, SHCoreDllService>()
      .RegisterSingleton<IKernel32DllService, Kernel32DllService>()
      .RegisterSingleton<IOleaccDllService, OleaccDllService>()
      .RegisterInstance<IAppResourcesService>(new AppResourcesService(Resources))
      .RegisterSingleton<GeneralHookService>()
      .RegisterSingleton<IKeyboardHookService, KeyboardHookService>()
      .RegisterSingleton<IClipboardHookService, ClipboardHookService>()
      .RegisterSingleton<IPasteWindowService, PasteWindowService>()
      .RegisterSingleton<IPasteService, PasteService>()
      .RegisterSingleton<IClipboardService, ClipboardService>()
      .RegisterSingleton<ISettingsService, SettingsService>()
      .RegisterSingleton<IPluginSettingsService>(p => p.Resolve<ISettingsService>())
      .RegisterSingleton<ITranslationService, TranslationService>()
      .RegisterSingleton<IThemeService, ThemeService>()
      .RegisterSingleton<IPluginCatalog, PluginCatalog>()
      .RegisterSingleton<IPluginsService, PluginsService>()
      .RegisterSingleton<IHttpClientFactory, HttpClientFactory>()
      .RegisterSingleton<InfoBarService>()
      .RegisterSingleton<IInfoBarSubscriber>(p => p.Resolve<InfoBarService>())
      .RegisterSingleton<IInfoBarService>(p => p.Resolve<InfoBarService>())
      .RegisterSingleton<IPinnedClipRepository, PinnedClipRepository>()
      .RegisterSingleton<IClipRepository, ClipRepository>()
      .RegisterSingleton<IPluginRepository, PluginRepository>()
      .RegisterSingleton<IInfoService, InfoService>()
      .RegisterSingleton<IGitHubClient>(cp =>
      {
        var infoService = cp.Resolve<IInfoService>();
        return new GitHubClient(new ProductHeaderValue("JustClipboardManager", infoService.InformationalVersion));
      })
      .Register<IKeyBindingRecordingService, KeyBindingRecordingService>()
      .Register<IUpdateService, UpdateService>()
      .Register<WshShell, WshShellWrapper>()
      .Register<IShortcutService, ShortcutService>()
      .RegisterShell<TrayIcon, TrayIconViewModel>()
      .RegisterShell<PasteWindow, PasteWindowViewModel>();

    if (Environment.OSVersion.Version >= Version.Parse("10.0.22000"))
    {
      containerRegistry.RegisterDialogWindow<MainDialogWindow>(WindowNames.MainAppWindow);
    }
    else
    {
      containerRegistry.RegisterDialogWindow<MainDialogBefore11Window>(WindowNames.MainAppWindow);
    }
    containerRegistry.RegisterDialogWindow<SimpleDialogWindow>(WindowNames.SimpleDialogWindow);
    containerRegistry.RegisterSingleInstanceDialog<MainDialog, MainDialogViewModel>(DialogNames.MainDialog);
    containerRegistry.RegisterDialog<UnregisteredHotkeysDialog, UnregisteredHotkeysDialogViewModel>(DialogNames.UnregisteredHotkeysDialog);
    containerRegistry.RegisterDialog<EditHotkeyDialog, EditHotkeyDialogViewModel>(DialogNames.EditHotkeyDialog);

    containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>(ViewNames.SettingsView);
    containerRegistry.RegisterForNavigation<SettingsGeneralView, SettingsGeneralViewModel>(ViewNames.SettingsGeneralView);
    containerRegistry.RegisterForNavigation<SettingsInterfaceView, SettingsInterfaceViewModel>(ViewNames.SettingsInterfaceView);
    containerRegistry.RegisterForNavigation<SettingsPasteWindowView, SettingsPasteWindowViewModel>(ViewNames.SettingsPasteWindowView);
    containerRegistry.RegisterForNavigation<SettingsHotkeysView, SettingsHotkeysViewModel>(ViewNames.SettingsHotkeysView);

    containerRegistry.RegisterForNavigation<PluginsView, PluginsViewModel>(ViewNames.PluginsView);
    containerRegistry.RegisterForNavigation<PluginsInstalledView, PluginsInstalledViewModel>(ViewNames.PluginsInstalledView);
    containerRegistry.RegisterForNavigation<PluginsSearchView, PluginsSearchViewModel>(ViewNames.PluginsSearchView);
    containerRegistry.RegisterForNavigation<PluginsSequenceView, PluginsSequenceViewModel>(ViewNames.PluginsSequenceView);

    containerRegistry.RegisterForNavigation<AboutView, AboutViewModel>(ViewNames.AboutView);
  }


  protected override Window? CreateShell()
  {
    return null;
  }


  protected override void InitializeModules()
  {
    // prevent modules auto-initialization
  }
}
