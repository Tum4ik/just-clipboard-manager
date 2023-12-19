using System.ComponentModel;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.PInvokeWrappers;

namespace Tum4ik.JustClipboardManager.UnitTests.Services;
public class ClipboardHookServiceTests
{
  private readonly IPasteWindowService _pasteWindowService = Substitute.For<IPasteWindowService>();
  private readonly IEventAggregator _eventAggregator = Substitute.For<IEventAggregator>();
  private readonly IUser32DllService _user32Dll = Substitute.For<IUser32DllService>();


  [Fact]
  internal void Constructor_AddClipboardListenerFailed_ThrowsException()
  {
    _user32Dll
      .AddClipboardFormatListener(Arg.Any<nint>())
      .Returns(false);
    var constructorCall = ()
      => new ClipboardHookService(_pasteWindowService, _eventAggregator, _user32Dll);
    constructorCall.Should().Throw<Win32Exception>();
  }


  [Fact]
  internal async Task HwndHook_ClipboardUpdateMsg_PublishesClipboardChangedEvent()
  {
    const nint WinHandle = 33;
    var clipboardChangedEvent = Substitute.For<ClipboardChangedEvent>();
    _pasteWindowService.WindowHandle.Returns(WinHandle);
    _user32Dll
      .AddClipboardFormatListener(WinHandle)
      .Returns(true);
    _eventAggregator.GetEvent<ClipboardChangedEvent>().Returns(clipboardChangedEvent);
    var testeeService = new ClipboardHookService(_pasteWindowService, _eventAggregator, _user32Dll);

    var handled = false;
    testeeService.HwndHook(WinHandle, 0x031D, default, default, ref handled);
    await Task.Delay(505);

    clipboardChangedEvent.Received(1).Publish();
  }


  [Fact]
  internal async Task HwndHook_ManySequentialClipboardUpdateMsgs_PublishesClipboardChangedEventOnlyForFirst()
  {
    const nint WinHandle = 33;
    var clipboardChangedEvent = Substitute.For<ClipboardChangedEvent>();
    _pasteWindowService.WindowHandle.Returns(WinHandle);
    _user32Dll
      .AddClipboardFormatListener(WinHandle)
      .Returns(true);
    _eventAggregator.GetEvent<ClipboardChangedEvent>().Returns(clipboardChangedEvent);
    var testeeService = new ClipboardHookService(_pasteWindowService, _eventAggregator, _user32Dll);

    var handled = false;
    for (var i = 0; i < 10; i++)
    {
      testeeService.HwndHook(WinHandle, 0x031D, default, default, ref handled);
    }
    await Task.Delay(505);

    clipboardChangedEvent.Received(1).Publish();
  }


  [Fact]
  internal void HwndHook_DestroyMsg_RemovesClipboardListener()
  {
    const nint WinHandle = 33;
    _pasteWindowService.WindowHandle.Returns(WinHandle);
    _user32Dll
      .AddClipboardFormatListener(WinHandle)
      .Returns(true);
    var testeeService = new ClipboardHookService(_pasteWindowService, _eventAggregator, _user32Dll);

    var handled = false;
    testeeService.HwndHook(WinHandle, 0x0002, default, default, ref handled);

    _user32Dll.Received(1).RemoveClipboardFormatListener(WinHandle);
  }
}
