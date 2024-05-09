using Microsoft.VisualStudio.Threading;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.Services;

// todo: re-implement using IEventAggregator
internal class InfoBarService : IInfoBarSubscriber, IInfoBarService
{
  private readonly JoinableTaskFactory _joinableTaskFactory;

  public InfoBarService(JoinableTaskFactory joinableTaskFactory)
  {
    _joinableTaskFactory = joinableTaskFactory;
  }


  public event Action<InfoBarPayload>? InfoReceived;


  public void ShowInfo(string body, string? title = null, Action<InfoBarResult>? callback = null)
  {
    Show(body, title, InfoBarSeverity.Informational, InfoBarActionType.None, null, callback);
  }

  public void ShowInfo(string body, InfoBarActionType actionType, string actionText, string? title = null, Action<InfoBarResult>? callback = null)
  {
    Show(body, title, InfoBarSeverity.Informational, actionType, actionText, callback);
  }

  public void ShowSuccess(string body, string? title = null, Action<InfoBarResult>? callback = null)
  {
    Show(body, title, InfoBarSeverity.Success, InfoBarActionType.None, null, callback);
  }

  public void ShowSuccess(string body, InfoBarActionType actionType, string actionText, string? title = null, Action<InfoBarResult>? callback = null)
  {
    Show(body, title, InfoBarSeverity.Success, actionType, actionText, callback);
  }

  public void ShowWarning(string body, string? title = null, Action<InfoBarResult>? callback = null)
  {
    Show(body, title, InfoBarSeverity.Warning, InfoBarActionType.None, null, callback);
  }

  public void ShowWarning(string body, InfoBarActionType actionType, string actionText, string? title = null, Action<InfoBarResult>? callback = null)
  {
    Show(body, title, InfoBarSeverity.Warning, actionType, actionText, callback);
  }

  public void ShowCritical(string body, string? title = null, Action<InfoBarResult>? callback = null)
  {
    Show(body, title, InfoBarSeverity.Critical, InfoBarActionType.None, null, callback);
  }

  public void ShowCritical(string body, InfoBarActionType actionType, string actionText, string? title = null, Action<InfoBarResult>? callback = null)
  {
    Show(body, title, InfoBarSeverity.Critical, actionType, actionText, callback);
  }


  public void Show(string body,
                   string? title,
                   InfoBarSeverity severity = InfoBarSeverity.Informational,
                   InfoBarActionType actionType = InfoBarActionType.None,
                   string? actionText = null,
                   Action<InfoBarResult>? callback = null)
  {
    var payload = new InfoBarPayload
    {
      Body = body,
      Title = title,
      Severity = severity,
      ActionType = actionType,
      ActionText = actionText,
      Callback = callback
    };
    _joinableTaskFactory.Run(async () =>
    {
      await _joinableTaskFactory.SwitchToMainThreadAsync();
      InfoReceived?.Invoke(payload);
    });
  }
}
