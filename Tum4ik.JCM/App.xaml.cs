// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using DryIoc;
using Tum4ik.JCM.Services;

namespace Tum4ik.JCM;
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
  }


  /// <summary>
  /// Registers types with the IoC container.
  /// </summary>
  /// <param name="registrator">The IoC container (registrator).</param>
  private static void RegisterTypes(IRegistrator registrator)
  {
    registrator.Register<IApplicationLifetime, App>(Reuse.Singleton);
  }


  public void ExitApplication()
  {
    _iocContainer?.Dispose();
    Exit();
  }
}
