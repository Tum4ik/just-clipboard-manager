namespace Tum4ik.JustClipboardManager.Services;
internal interface IInfoService
{
  string ProductName { get; }
  string InformationalVersion { get; }
  Version Version { get; }
}
