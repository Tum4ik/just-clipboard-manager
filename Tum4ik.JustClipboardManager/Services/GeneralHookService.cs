using System.Windows.Interop;

namespace Tum4ik.JustClipboardManager.Services;
internal sealed class GeneralHookService : IDisposable
{
  private readonly IKeyboardHookService _keyboardHookService;
  private readonly IClipboardHookService _clipboardHookService;

  public GeneralHookService(IPasteWindowService pasteWindowService,
                            IKeyboardHookService keyboardHookService,
                            IClipboardHookService clipboardHookService)
  {
    
    _keyboardHookService = keyboardHookService;
    _clipboardHookService = clipboardHookService;

    _hwndSource = HwndSource.FromHwnd(pasteWindowService.WindowHandle);
    _hwndSource.AddHook(_keyboardHookService.HwndHook);
    _hwndSource.AddHook(_clipboardHookService.HwndHook);
  }


  private readonly HwndSource _hwndSource;
  

  public void Dispose()
  {
    _hwndSource.RemoveHook(_keyboardHookService.HwndHook);
    _hwndSource.RemoveHook(_clipboardHookService.HwndHook);
    _hwndSource.Dispose();
  }
}
