namespace Tum4ik.JustClipboardManager.Services;
internal interface IAppEnvironmentService
{
  AppEnvironment Environment { get; }
}


internal enum AppEnvironment
{
  Production,
  Development,
  UiTest
}
