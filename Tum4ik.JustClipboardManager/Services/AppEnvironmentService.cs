namespace Tum4ik.JustClipboardManager.Services;
internal class AppEnvironmentService : IAppEnvironmentService
{
  public AppEnvironmentService(AppEnvironment environment)
  {
    Environment = environment;
  }


  public AppEnvironment Environment { get; }
}
