using System.ComponentModel;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.PInvoke;

namespace Tum4ik.JustClipboardManager.UnitTests.Services;
public class ClipboardHookServiceTests
{
  private readonly Mock<IPasteWindowService> _pasteWindowServiceMock = new();
  private readonly Mock<IEventAggregator> _eventAggregatorMock = new();
  private readonly Mock<IUser32DllService> _user32DllMock = new();


  [Fact]
  internal void Constructor_AddClipboardListenerFailed_ThrowsException()
  {
    _user32DllMock
      .Setup(u32 => u32.AddClipboardFormatListener(It.IsAny<nint>()))
      .Returns(false);
    var constructorCall = ()
      => new ClipboardHookService(_pasteWindowServiceMock.Object, _eventAggregatorMock.Object, _user32DllMock.Object);
    constructorCall.Should().Throw<Win32Exception>();
  }


  [Fact]
  internal async Task HwndHook_ClipboardUpdateMsg_PublishesClipboardChangedEvent()
  {
    const nint WinHandle = 33;
    var clipboardChangedEventMock = new Mock<ClipboardChangedEvent>();
    _pasteWindowServiceMock.Setup(pws => pws.WindowHandle).Returns(WinHandle);
    _user32DllMock
      .Setup(u32 => u32.AddClipboardFormatListener(WinHandle))
      .Returns(true);
    _eventAggregatorMock.Setup(ea => ea.GetEvent<ClipboardChangedEvent>()).Returns(clipboardChangedEventMock.Object);
    var testeeService = new ClipboardHookService(
      _pasteWindowServiceMock.Object, _eventAggregatorMock.Object, _user32DllMock.Object
    );

    var handled = false;
    testeeService.HwndHook(WinHandle, 0x031D, default, default, ref handled);
    await Task.Delay(501);

    clipboardChangedEventMock.Verify(cce => cce.Publish(), Times.Once());
  }


  [Fact]
  internal async Task HwndHook_ManySequentialClipboardUpdateMsgs_PublishesClipboardChangedEventOnlyForFirst()
  {
    const nint WinHandle = 33;
    var clipboardChangedEventMock = new Mock<ClipboardChangedEvent>();
    _pasteWindowServiceMock.Setup(pws => pws.WindowHandle).Returns(WinHandle);
    _user32DllMock
      .Setup(u32 => u32.AddClipboardFormatListener(WinHandle))
      .Returns(true);
    _eventAggregatorMock.Setup(ea => ea.GetEvent<ClipboardChangedEvent>()).Returns(clipboardChangedEventMock.Object);
    var testeeService = new ClipboardHookService(
      _pasteWindowServiceMock.Object, _eventAggregatorMock.Object, _user32DllMock.Object
    );

    var handled = false;
    for (var i = 0; i < 10; i++)
    {
      testeeService.HwndHook(WinHandle, 0x031D, default, default, ref handled);
    }
    await Task.Delay(501);

    clipboardChangedEventMock.Verify(cce => cce.Publish(), Times.Once());
  }


  [Fact]
  internal async Task HwndHook_DestroyMsg_RemovesClipboardListener()
  {
    const nint WinHandle = 33;
    _pasteWindowServiceMock.Setup(pws => pws.WindowHandle).Returns(WinHandle);
    _user32DllMock
      .Setup(u32 => u32.AddClipboardFormatListener(WinHandle))
      .Returns(true);
    var testeeService = new ClipboardHookService(
      _pasteWindowServiceMock.Object, _eventAggregatorMock.Object, _user32DllMock.Object
    );

    var handled = false;
    testeeService.HwndHook(WinHandle, 0x0002, default, default, ref handled);

    _user32DllMock.Verify(u32 => u32.RemoveClipboardFormatListener(WinHandle), Times.Once());
  }
}
