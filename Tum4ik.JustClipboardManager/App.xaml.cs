using System.Windows;
using System.Windows.Threading;
using DryIoc;
using IWshRuntimeLibrary;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Octokit;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using SingleInstanceCore;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Data;
using Tum4ik.JustClipboardManager.Data.Repositories;
using Tum4ik.JustClipboardManager.Extensions;
using Tum4ik.JustClipboardManager.Ioc.Wrappers;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Properties;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Dialogs;
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
    var restartAfterCrashArg = args.FirstOrDefault(a => a.StartsWith(RestartAfterCrashArg, StringComparison.Ordinal));
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

    app.DispatcherUnhandledException += OnUnhandledException;
    UpgradeSettings();
    app.InitializeComponent();
    app.OverrideDefaultProperties();
    app.Run();
  }


  private static void OnUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
  {
    // TODO: improve to give user a chance to decide send or not
    // TODO: and also notify user about the problem anyway
    Crashes.TrackError(e.Exception, new Dictionary<string, string>
    {
      { "Message", "Unhandled Exception" },
      { "OS Architecture", Environment.Is64BitOperatingSystem ? "x64" : "x86" },
      { "App Architecture", Environment.Is64BitProcess ? "x64" : "x86" }
    });
    Task.Delay(10000).Wait(); // Give Crashes some time to be able to record exception properly
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
      Process.Start(new ProcessStartInfo(processPath)
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
      SettingsGeneral.Default.Upgrade();
      SettingsHotkeys.Default.Upgrade();
      SettingsInterface.Default.Upgrade();
      InternalSettings.Default.IsSettingsUpgradeRequired = false;
      InternalSettings.Default.Save();
    }
  }


  public void OnInstanceInvoked(string[] args)
  {
    // TODO: maybe show "already started" notification
  }


  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    var configuration = Container.Resolve<IConfiguration>();
    AppCenter.Start(configuration["MicrosoftAppCenterSecret"], typeof(Crashes), typeof(Analytics));

    var updateService = Container.Resolve<IUpdateService>();
    updateService.SilentUpdate();

    using var dbContext = Container.Resolve<IDbContextFactory<AppDbContext>>().CreateDbContext();
    dbContext.Database.Migrate();

    RemoveOldClips();
    var trayIcon = Container.Resolve<TrayIcon>();
    var hookService = Container.Resolve<GeneralHookService>();
  }


  protected override void OnExit(ExitEventArgs e)
  {
    Container.GetContainer().Dispose();
    SingleInstance.Cleanup();

    base.OnExit(e);
  }


  private void RemoveOldClips()
  {
    var clipRepository = Container.Resolve<IClipRepository>();
    clipRepository.DeleteBeforeDateAsync(DateTime.Now.AddMonths(-3)).Await(e => Crashes.TrackError(e)); // TODO: before date from settings
  }


  protected override void RegisterTypes(IContainerRegistry containerRegistry)
  {
    containerRegistry
      .RegisterConfiguration()
      .RegisterGeneratedWrappers()
      .RegisterDatabase()
      .RegisterSingleton<IDialogService, ExtendedDialogService>()
      .RegisterSingleton<IUser32DllService, User32DllService>()
      .RegisterSingleton<ISHCoreDllService, SHCoreDllService>()
      .RegisterSingleton<IKernel32DllService, Kernel32DllService>()
      .RegisterInstance<IAppResourcesService>(new AppResourcesService(Resources))
      .RegisterSingleton<GeneralHookService>()
      .RegisterSingleton<IKeyboardHookService, KeyboardHookService>()
      .RegisterSingleton<IClipboardHookService, ClipboardHookService>()
      .RegisterSingleton<IPasteWindowService, PasteWindowService>()
      .RegisterSingleton<IPasteService, PasteService>()
      .RegisterSingleton<IClipboardService, ClipboardService>()
      .RegisterSingleton<ISettingsService, SettingsService>()
      .RegisterSingleton<ITranslationService, TranslationService>()
      .RegisterSingleton<IThemeService, ThemeService>()
      .RegisterSingleton<IPluginsService, PluginsService>()
      .Register<IKeyBindingRecordingService, KeyBindingRecordingService>()
      .Register<IClipRepository, ClipRepository>()
      .Register<IInfoService, InfoService>()
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

    containerRegistry.RegisterDialogWindow<MainDialogWindow>(WindowNames.MainAppWindow);
    containerRegistry.RegisterDialogWindow<SimpleDialogWindow>(WindowNames.SimpleDialogWindow);
    containerRegistry.RegisterSingleInstanceDialog<MainDialog, MainDialogViewModel>(DialogNames.MainDialog);
    containerRegistry.RegisterDialog<UnregisteredHotkeysDialog, UnregisteredHotkeysDialogViewModel>(DialogNames.UnregisteredHotkeysDialog);
    containerRegistry.RegisterDialog<EditHotkeyDialog, EditHotkeyDialogViewModel>(DialogNames.EditHotkeyDialog);

    containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>(ViewNames.SettingsView);
    containerRegistry.RegisterForNavigation<SettingsGeneralView, SettingsGeneralViewModel>(ViewNames.SettingsGeneralView);
    containerRegistry.RegisterForNavigation<SettingsInterfaceView, SettingsInterfaceViewModel>(ViewNames.SettingsInterfaceView);
    containerRegistry.RegisterForNavigation<SettingsHotkeysView, SettingsHotkeysViewModel>(ViewNames.SettingsHotkeysView);
    containerRegistry.RegisterForNavigation<AboutView, AboutViewModel>(ViewNames.AboutView);
  }


  protected override Window? CreateShell()
  {
    return null;
  }


  protected override IModuleCatalog CreateModuleCatalog()
  {
    return new DirectoryModuleCatalog { ModulePath = "Plugins" };
  }
}
