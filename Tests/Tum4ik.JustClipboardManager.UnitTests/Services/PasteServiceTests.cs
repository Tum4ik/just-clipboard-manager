using System.Windows.Input;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.PInvoke;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

namespace Tum4ik.JustClipboardManager.UnitTests.Services;
public class PasteServiceTests
{
  private readonly IClipboardService _clipboardService = Substitute.For<IClipboardService>();
  private readonly IUser32DllService _user32Dll = Substitute.For<IUser32DllService>();
  private readonly PasteService _testeeService;

  public PasteServiceTests()
  {
    _testeeService = new(_clipboardService, _user32Dll);
  }


  [Fact]
  internal void PasteData_DataIsEmpty_NothingToDo()
  {
    _testeeService.PasteData(nint.Zero, new List<FormattedDataObject>(), null);
    _clipboardService.ReceivedCalls().Any().Should().BeFalse();
    _user32Dll.ReceivedCalls().Any().Should().BeFalse();
  }


  [Fact]
  internal void PasteData_DataIsPresent_DataIsInsertedIntoClipboardAndCtrlVHotkeyIsEmulated()
  {
    const nint TargetWindowPtr = 42;
    var data = new List<FormattedDataObject>
    {
      new()
      {
        Data = Array.Empty<byte>(),
        DataDotnetType = "int",
        Format = "number",
        FormatOrder = 0
      }
    };
    var additionalInfo = "info";

    _testeeService.PasteData(TargetWindowPtr, data, additionalInfo);

    _clipboardService.Received(1).Paste(data, additionalInfo);
    _user32Dll.Received(1).SetForegroundWindow(TargetWindowPtr);
    _user32Dll.Received(1).SetFocus(TargetWindowPtr);
    _user32Dll.Received(1).SendInput(4, IsCtrlVInput(), InputStructSize());
  }


  private static ref INPUT[] IsCtrlVInput()
  {
    return ref Arg.Is<INPUT[]>(inputs =>
      inputs.Length == 4
      &&
      inputs[0].type == INPUTTYPE.INPUT_KEYBOARD
        && inputs[0].data.ki.wVk == KeyInterop.VirtualKeyFromKey(Key.LeftCtrl)
        && inputs[0].data.ki.dwFlags == default
      &&
      inputs[1].type == INPUTTYPE.INPUT_KEYBOARD
        && inputs[1].data.ki.wVk == KeyInterop.VirtualKeyFromKey(Key.V)
        && inputs[1].data.ki.dwFlags == default
      &&
      inputs[2].type == INPUTTYPE.INPUT_KEYBOARD
        && inputs[2].data.ki.wVk == KeyInterop.VirtualKeyFromKey(Key.V)
        && inputs[2].data.ki.dwFlags == KEYEVENT.KEYEVENTF_KEYUP
      &&
      inputs[3].type == INPUTTYPE.INPUT_KEYBOARD
        && inputs[3].data.ki.wVk == KeyInterop.VirtualKeyFromKey(Key.LeftCtrl)
        && inputs[3].data.ki.dwFlags == KEYEVENT.KEYEVENTF_KEYUP);
  }

  private static unsafe ref int InputStructSize()
  {
    var requiredSize = sizeof(INPUT);
    return ref Arg.Is<int>(size => size == requiredSize);
  }
}
