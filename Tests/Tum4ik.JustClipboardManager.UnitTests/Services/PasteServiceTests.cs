using System.Windows.Input;
using Tum4ik.JustClipboardManager.Data.Models;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.PInvoke;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

namespace Tum4ik.JustClipboardManager.UnitTests.Services;
public class PasteServiceTests
{
  private readonly Mock<IClipboardService> _clipboardServiceMock = new();
  private readonly Mock<IUser32DllService> _user32DllMock = new();
  private readonly PasteService _testeeService;

  public PasteServiceTests()
  {
    _testeeService = new(_clipboardServiceMock.Object, _user32DllMock.Object);
  }


  [Fact]
  internal void PasteData_DataIsEmpty_NothingToDo()
  {
    _testeeService.PasteData(nint.Zero, new List<FormattedDataObject>());
    _clipboardServiceMock.VerifyNoOtherCalls();
    _user32DllMock.VerifyNoOtherCalls();
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

    _testeeService.PasteData(TargetWindowPtr, data);

    _clipboardServiceMock.Verify(cs => cs.Paste(data), Times.Once);
    _user32DllMock.Verify(u32 => u32.SetForegroundWindow(TargetWindowPtr), Times.Once);
    _user32DllMock.Verify(u32 => u32.SetFocus(TargetWindowPtr), Times.Once);
    _user32DllMock.Verify(u32 => u32.SendInput(4, IsCtrlVInput(), InputStructSize()), Times.Once);
  }


  private static INPUT[] IsCtrlVInput()
  {
    return Match.Create<INPUT[]>(inputs =>
    {
      var lengthCondition = inputs.Length == 4;
      var input0Condition = inputs[0].type == INPUTTYPE.INPUT_KEYBOARD
        && inputs[0].data.ki.wVk == KeyInterop.VirtualKeyFromKey(Key.LeftCtrl)
        && inputs[0].data.ki.dwFlags == default;
      var input1Condition = inputs[1].type == INPUTTYPE.INPUT_KEYBOARD
        && inputs[1].data.ki.wVk == KeyInterop.VirtualKeyFromKey(Key.V)
        && inputs[1].data.ki.dwFlags == default;
      var input2Condition = inputs[2].type == INPUTTYPE.INPUT_KEYBOARD
        && inputs[2].data.ki.wVk == KeyInterop.VirtualKeyFromKey(Key.V)
        && inputs[2].data.ki.dwFlags == KEYEVENT.KEYEVENTF_KEYUP;
      var input3Condition = inputs[3].type == INPUTTYPE.INPUT_KEYBOARD
        && inputs[3].data.ki.wVk == KeyInterop.VirtualKeyFromKey(Key.LeftCtrl)
        && inputs[3].data.ki.dwFlags == KEYEVENT.KEYEVENTF_KEYUP;
      return lengthCondition && input0Condition && input1Condition && input2Condition && input3Condition;
    });
  }

  private static unsafe int InputStructSize()
  {
    return Match.Create<int>(size => size == sizeof(INPUT));
  }
}
