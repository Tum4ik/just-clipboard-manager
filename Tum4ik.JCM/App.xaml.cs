// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using DryIoc;
using Tum4ik.JustClipboardManager.Controls;
using Tum4ik.JustClipboardManager.PluginDevKit;
using Tum4ik.JustClipboardManager.Plugins;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.ViewModels;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
  "CA1001:Types that own disposable fields should be disposable",
  Justification = "Disposalbes are handled by ExitApplication method."
)]
public partial class App : Application, IApplicationLifetime
{
  /// <summary>
  /// Initializes the singleton application object. This is the first line of authored code
  /// executed, and as such is the logical equivalent of main() or WinMain().
  /// </summary>
  public App()
  {
    InitializeComponent();
  }

  private Container? _iocContainer;

  /// <summary>
  /// Invoked when the application is launched.
  /// </summary>
  /// <param name="args">Details about the launch request and process.</param>
  protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
  {
    CreateIocContainer();
  }


  private void CreateIocContainer()
  {
    _iocContainer = new Container();
    RegisterTypes(_iocContainer);
    LoadPlugins(_iocContainer);

    //var tP = _iocContainer.Resolve<IPlugin>("D930D2CD-3FD9-4012-A363-120676E22AFA");
    //var dt = tP.RepresentationDataDataTemplate;
    _iocContainer.Register<PasteWindow>();
    _iocContainer.Register<PasteWindowViewModel>();

    var wind = _iocContainer.Resolve<PasteWindow>();
    wind.Activate();
  }


  /// <summary>
  /// Registers types with the IoC container.
  /// </summary>
  /// <param name="registrator">The IoC container (registrator).</param>
  private static void RegisterTypes(IRegistrator registrator)
  {
    registrator.Register<IApplicationLifetime, App>(Reuse.Singleton);
    registrator.Register<IPluginLoader, PluginLoader>(Reuse.Singleton);
    registrator.Register<ClipTypeDataTemplateSelector>(Reuse.Singleton);
  }


  private static void LoadPlugins(IResolver resolver)
  {
    var pluginLoader = resolver.Resolve<IPluginLoader>();
    pluginLoader.Load();
  }


  public void ExitApplication()
  {
    _iocContainer?.Dispose();
    Exit();
  }
}
