using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using DryIoc;
using IWshRuntimeLibrary;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Threading;
using Octokit;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
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
    var app = new App();

    var instanceUniqueName = "JustClipboardManager_B9D1525B-D41C-49E0-83F7-038339056F46";
#if DEBUG
    instanceUniqueName += "_Development";
#endif
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

#if !DEBUG
    app.DispatcherUnhandledException += OnUnhandledException;
#endif
    UpgradeSettings();
    app.InitializeComponent();
    app.OverrideDefaultProperties();
    app.Run();
  }


  private readonly IConfiguration _configuration;


  public App()
  {
    _configuration = ConfigurationHelper.CreateConfiguration();
    SentrySdk.Init(o =>
    {
      o.Dsn = _configuration["SentryDsn"];
      o.AutoSessionTracking = true;
      o.IsGlobalModeEnabled = true;
      o.Environment = _configuration["SentryEnvironment"];
    });
  }


  private static void OnUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
  {
    // TODO: notify user about the problem anyway
    SentrySdk.CaptureException(e.Exception, scope => scope.AddBreadcrumb("Unhandled Exception"));
    e.Handled = true;
    RestartApp();
    Current.Shutdown();
  }


  private static void RestartApp()
  {
    var processPath = Environment.ProcessPath;
    if (processPath is not null && RestartAfterCrashCount < 5)
    {
#if !DEBUG
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(processPath)
      {
        Arguments = $"{RestartAfterCrashArg}{RestartAfterCrashDelimiter}{RestartAfterCrashCount + 1}",
        UseShellExecute = true
      });
#endif
    }
  }


  private void OverrideDefaultProperties()
  {
    FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
    {
      DefaultValue = FindResource(typeof(Window))
    });
  }


  private static void UpgradeSettings()
  {
    if (InternalSettings.Default.IsSettingsUpgradeRequired)
    {
      InternalSettings.Default.Upgrade();
      PluginSettings.Default.Upgrade();
      SettingsGeneral.Default.Upgrade();
      SettingsHotkeys.Default.Upgrade();
      SettingsInterface.Default.Upgrade();
      SettingsPasteWindow.Default.Upgrade();

      InternalSettings.Default.IsSettingsUpgradeRequired = false;
      InternalSettings.Default.Save();
    }
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


  protected override void OnStartup(StartupEventArgs e)
  {
    try
    {
      // Important to call before base.OnStartup(e).
      // Otherwise the plugins will be initialized and it will not be possible to remove files.
      RemoveFilesOfDeletedPlugins();
      UpdatePluginsIfForced();
    }
    catch (Exception ex)
    {
      SentrySdk.CaptureException(ex, scope => scope.AddBreadcrumb("Error during removing files before startup."));
    }

    try
    {
      base.OnStartup(e);
    }
    catch (ModuleInitializeException ex)
    {
      SentrySdk.CaptureException(ex);
    }
    
    var updateService = Container.Resolve<IUpdateService>();
    var joinableTaskFactory = Container.Resolve<JoinableTaskFactory>();
    var result = joinableTaskFactory.Run(updateService.SilentUpdateAsync);
    if (result == UpdateResult.Started)
    {
#if !DEBUG
      return;
#endif
    }

    using (var dbContext = Container.Resolve<IDbContextFactory<AppDbContext>>().CreateDbContext())
    {
      try
      {
        dbContext.Database.Migrate();
      }
      catch (SqliteException)
      {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();
      }
    }

    var settingsService = Container.Resolve<ISettingsService>();
    var clipRepository = Container.Resolve<IClipRepository>();
    var pluginsService = Container.Resolve<IPluginsService>();
    var sentryHub = Container.Resolve<Lazy<IHub>>();

    RemoveOldClipsAsync(settingsService, clipRepository).Await(e => sentryHub.Value.CaptureException(e));
    PreInstallPluginsAsync(pluginsService).Await(e => sentryHub.Value.CaptureException(e));
    var trayIcon = Container.Resolve<TrayIcon>();
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


  private static async Task PreInstallPluginsAsync(IPluginsService pluginsService)
  {
    const string FileName = "pre-install-plugins";
    if (!System.IO.File.Exists(FileName))
    {
      return;
    }

    var pluginIdsToInstall = await System.IO.File.ReadAllLinesAsync(FileName).ConfigureAwait(false);
    await foreach (var pluginDto in pluginsService.SearchPluginsAsync().ConfigureAwait(false))
    {
      if (pluginIdsToInstall.Contains(pluginDto.Id))
      {
        await pluginsService.InstallPluginAsync(pluginDto.DownloadLink, pluginDto.Id).ConfigureAwait(false);
      }
    }
    System.IO.File.Delete(FileName);
  }


  private static void RemoveFilesOfDeletedPlugins()
  {
    const string FileName = PluginsService.PluginFilesToRemoveFileName;
    if (System.IO.File.Exists(FileName))
    {
      foreach (var line in System.IO.File.ReadLines(FileName).Where(System.IO.File.Exists))
      {
        System.IO.File.Delete(line);
      }
      System.IO.File.Delete(FileName);
    }
  }


  private static void UpdatePluginsIfForced()
  {
    // todo: optimize
    if (!System.IO.File.Exists("force-plugins-update"))
    {
      return;
    }

    try
    {
      using (var pluginFilesStream = new FileStream(PluginsService.PluginsJsonFileName, System.IO.FileMode.OpenOrCreate))
      {
        var pluginIdToPluginFiles = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(pluginFilesStream);
        if (pluginIdToPluginFiles is null)
        {
          return;
        }

        using var preInstalledPluginsWriter = System.IO.File.AppendText("pre-install-plugins");
        foreach (var (pluginId, pluginFiles) in pluginIdToPluginFiles)
        {
          preInstalledPluginsWriter.WriteLine(pluginId);
          foreach (var pluginFile in pluginFiles)
          {
            System.IO.File.Delete(pluginFile);
          }
        }
      }
    }
    catch (JsonException)
    {
      // suppress
    }
    finally
    {
      System.IO.File.Delete("force-plugins-update");
      System.IO.File.Delete(PluginsService.PluginsJsonFileName);
    }
  }


  protected override void RegisterTypes(IContainerRegistry containerRegistry)
  {
    containerRegistry
      .RegisterGeneratedWrappers()
      .RegisterDatabase()
      .RegisterThreadSwitching()
      .RegisterInstance(_configuration)
      .RegisterInstance<IApplicationLifetime>(this)
      .RegisterSingleton<IHub>(() => HubAdapter.Instance)
      .RegisterSingleton<ILoadableDirectoryModuleCatalog>(p => p.Resolve<IModuleCatalog>())
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
      .RegisterSingleton<IPluginsService, PluginsService>()
      .RegisterSingleton<IPluginsRegistryService>(p => p.Resolve<IPluginsService>())
      .RegisterSingleton<IHttpClientFactory, HttpClientFactory>()
      .RegisterSingleton<InfoBarService>()
      .RegisterSingleton<IInfoBarSubscriber>(p => p.Resolve<InfoBarService>())
      .RegisterSingleton<IInfoBarService>(p => p.Resolve<InfoBarService>())
      .RegisterSingleton<IClipRepository, ClipRepository>()
      .RegisterSingleton<IInfoService, InfoService>()
      .Register<IKeyBindingRecordingService, KeyBindingRecordingService>()
      .Register<IUpdateService, UpdateService>()
      .Register<IGitHubClient>(cp =>
      {
        var infoService = cp.Resolve<IInfoService>();
        return new GitHubClient(new ProductHeaderValue("JustClipboardManager", infoService.InformationalVersion));
      })
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


  protected override IModuleCatalog CreateModuleCatalog()
  {
    return new LoadableDirectoryModuleCatalog { ModulePath = "./" };
  }
}
