using JustClipboardManager.Application.IPC.Events;

namespace JustClipboardManager.Application.Services;

public interface IWindowingService
{
  void ShowPasteWindow();

  void ShowMainWindow();
  Task PublishEventForMainWindowAsync(AppEvent appEvent);
}
