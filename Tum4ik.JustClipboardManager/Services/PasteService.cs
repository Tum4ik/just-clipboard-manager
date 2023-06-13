using System.Windows.Input;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services.PInvoke;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

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


  public void PasteData(nint targetWindowPtr, ICollection<FormattedDataObject> data)
  {
    if (data.Count == 0)
    {
      return;
    }

    _clipboardService.Paste(data);
    _user32Dll.SetForegroundWindow(targetWindowPtr);
    _user32Dll.SetFocus(targetWindowPtr);

    var ctrl = (ushort) KeyInterop.VirtualKeyFromKey(Key.LeftCtrl);
    var v = (ushort) KeyInterop.VirtualKeyFromKey(Key.V);

    var inputs = new INPUT[4];
    inputs[0].type = inputs[1].type = inputs[2].type = inputs[3].type = INPUTTYPE.INPUT_KEYBOARD;

    inputs[0].data.ki.wVk = ctrl;
    inputs[1].data.ki.wVk = v;

    inputs[2].data.ki.wVk = v;
    inputs[2].data.ki.dwFlags = KEYEVENT.KEYEVENTF_KEYUP;

    inputs[3].data.ki.wVk = ctrl;
    inputs[3].data.ki.dwFlags = KEYEVENT.KEYEVENTF_KEYUP;

    _user32Dll.SendInput((uint) inputs.Length, inputs, InputStructSize);
  }
}
