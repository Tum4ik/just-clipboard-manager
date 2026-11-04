using JustClipboardManager.Application.Common.Abstractions;

namespace JustClipboardManager.Application.Services;

public interface IWindowingService
{
  void ShowPasteWindow();

  void ShowMainWindow();
  Task PublishEventForMainWindowAsync<TEvent>(TEvent appEvent) where TEvent : IAppEvent, new();
}
