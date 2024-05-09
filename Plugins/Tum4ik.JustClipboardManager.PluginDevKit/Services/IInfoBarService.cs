namespace Tum4ik.JustClipboardManager.PluginDevKit.Services;
public interface IInfoBarService
{
  void ShowInfo(string body, string? title = null, Action<InfoBarResult>? callback = null);
  void ShowInfo(string body, InfoBarActionType actionType, string actionText, string? title = null, Action<InfoBarResult>? callback = null);
  void ShowSuccess(string body, string? title = null, Action<InfoBarResult>? callback = null);
  void ShowSuccess(string body, InfoBarActionType actionType, string actionText, string? title = null, Action<InfoBarResult>? callback = null);
  void ShowWarning(string body, string? title = null, Action<InfoBarResult>? callback = null);
  void ShowWarning(string body, InfoBarActionType actionType, string actionText, string? title = null, Action<InfoBarResult>? callback = null);
  void ShowCritical(string body, string? title = null, Action<InfoBarResult>? callback = null);
  void ShowCritical(string body, InfoBarActionType actionType, string actionText, string? title = null, Action<InfoBarResult>? callback = null);

  void Show(string body,
            string? title,
            InfoBarSeverity severity = InfoBarSeverity.Informational,
            InfoBarActionType actionType = InfoBarActionType.None,
            string? actionText = null,
            Action<InfoBarResult>? callback = null);
}


public enum InfoBarSeverity
{
  Informational, Success, Warning, Critical
}


public enum InfoBarActionType
{
  None, /*HyperlinkButton,*/ Button
}


public enum InfoBarResult
{
  Cancel, Action
}
