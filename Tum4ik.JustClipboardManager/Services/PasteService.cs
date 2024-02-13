using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Tum4ik.JustClipboardManager.Services;
internal class PasteService : IPasteService
{
  private readonly IClipboardService _clipboardService;
  private readonly IUser32DllService _user32Dll;

  public PasteService(IClipboardService clipboardService,
                      IUser32DllService user32Dll)
  {
    _clipboardService = clipboardService;
    _user32Dll = user32Dll;
  }


  private static readonly unsafe int InputStructSize = sizeof(INPUT);


  public unsafe void PasteData(nint targetWindowPtr, ICollection<FormattedDataObject> data, string? additionalInfo)
  {
    if (data.Count == 0)
    {
      return;
    }

    _clipboardService.Paste(data, additionalInfo);
    _user32Dll.SetForegroundWindow(targetWindowPtr);
    _user32Dll.SetFocus(targetWindowPtr);

    var ctrl = VIRTUAL_KEY.VK_LCONTROL;
    var v = VIRTUAL_KEY.VK_V;

    Span<INPUT> inputs = stackalloc INPUT[4];
    inputs[0].type = inputs[1].type = inputs[2].type = inputs[3].type = INPUT_TYPE.INPUT_KEYBOARD;

    inputs[0].Anonymous.ki.wVk = ctrl;
    inputs[1].Anonymous.ki.wVk = v;

    inputs[2].Anonymous.ki.wVk = v;
    inputs[2].Anonymous.ki.dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP;

    inputs[3].Anonymous.ki.wVk = ctrl;
    inputs[3].Anonymous.ki.dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP;

    _user32Dll.SendInput(inputs, InputStructSize);
  }
}
