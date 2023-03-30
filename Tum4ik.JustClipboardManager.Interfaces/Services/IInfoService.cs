namespace Tum4ik.JustClipboardManager.Interfaces.Services;
public interface IInfoService
{
  string ProductName { get; }
  string InformationalVersion { get; }
  Version Version { get; }
}
