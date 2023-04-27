using System.Windows;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.Services.Dialogs;
using Tum4ik.JustClipboardManager.Services.PInvoke;
using Tum4ik.JustClipboardManager.Services.PInvoke.ParameterModels;

namespace Tum4ik.JustClipboardManager.UnitTests.Services.Dialogs;
public class ExtendedDialogServiceTests
{
  private readonly Mock<IContainerExtension> _containerExtensionMock = new();
  private readonly Mock<IUser32DllService> _user32DllMock = new();
  private readonly Mock<IDialogWindowExtended> _dialogWindowExtendedMock = new();
  private readonly Mock<IDialogAware> _dialogViewModelMock = new();
  private readonly ExtendedDialogService _testeeService;

  public ExtendedDialogServiceTests()
  {
    _testeeService = new(_containerExtensionMock.Object, _user32DllMock.Object);
  }


  public static IEnumerable<object?[]> ShowAndShowDialogData(bool windowIsActive)
  {
    foreach (var windowState in Enum.GetValues<WindowState>())
    {
      Action<ExtendedDialogService, string, IDialogParameters, Action<IDialogResult>, string?> action1 =
        (service, name, parameters, callback, windowName) => service.Show(name, parameters, callback);
      yield return new object?[] { action1, null, windowState, windowIsActive };

      Action<ExtendedDialogService, string, IDialogParameters, Action<IDialogResult>, string?> action2 =
        (service, name, parameters, callback, windowName) => service.ShowDialog(name, parameters, callback);
      yield return new object?[] { action2, null, windowState, windowIsActive };

      Action<ExtendedDialogService, string, IDialogParameters, Action<IDialogResult>, string?> action3 =
        (service, name, parameters, callback, windowName) => service.Show(name, parameters, callback, windowName);
      yield return new object?[] { action3, "DialogWindowName", windowState, windowIsActive };

      Action<ExtendedDialogService, string, IDialogParameters, Action<IDialogResult>, string?> action4 =
        (service, name, parameters, callback, windowName) => service.ShowDialog(name, parameters, callback, windowName);
      yield return new object?[] { action4, "DialogWindowName", windowState, windowIsActive };
    }
  }


  [Theory]
  [MemberData(nameof(ShowAndShowDialogData), true)]
  [MemberData(nameof(ShowAndShowDialogData), false)]
  internal void ShowAndShowDialog_NotSingleInstance_SpecialBehaviorIsNotApplied(
    Action<ExtendedDialogService, string, IDialogParameters, Action<IDialogResult>, string?> show,
    string? dialogWindowName,
    WindowState windowState,
    bool windowIsActive
  )
  {
    var callbackCalled = false;
    var dialogName = SetupDialog(dialogWindowName, false, windowState, windowIsActive);

    var thread = new Thread(() =>
    {
      var dialogContent = new FrameworkElement
      {
        DataContext = _dialogViewModelMock.Object
      };
      _containerExtensionMock
        .Setup(ce => ce.Resolve(typeof(object), dialogName))
        .Returns(dialogContent);
      show(_testeeService, dialogName, new DialogParameters(), r => callbackCalled = true, dialogWindowName);
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();

    _dialogWindowExtendedMock.Raise(dw => dw.Closed += null, EventArgs.Empty);

    callbackCalled.Should().BeTrue();
    _dialogViewModelMock.Verify(vm => vm.OnDialogOpened(It.IsAny<IDialogParameters>()), Times.Once);
    _dialogWindowExtendedMock.Verify(dw => dw.Activate(), Times.Never);
    _user32DllMock.Verify(u => u.ShowWindow(It.IsAny<nint>(), It.IsAny<ShowWindowCommand>()), Times.Never);
  }


  [Theory]
  [MemberData(nameof(ShowAndShowDialogData), true)]
  [MemberData(nameof(ShowAndShowDialogData), false)]
  internal void ShowAndShowDialog_SingleInstanceNotOpen_SpecialBehaviorIsNotApplied(
    Action<ExtendedDialogService, string, IDialogParameters, Action<IDialogResult>, string?> show,
    string? dialogWindowName,
    WindowState windowState,
    bool windowIsActive
  )
  {
    var callbackCalled = false;
    var dialogName = SetupDialog(dialogWindowName, true, windowState, windowIsActive);

    var thread = new Thread(() =>
    {
      var dialogContent = new FrameworkElement
      {
        DataContext = _dialogViewModelMock.Object
      };
      _containerExtensionMock
        .Setup(ce => ce.Resolve(typeof(object), dialogName))
        .Returns(dialogContent);
      show(_testeeService, dialogName, new DialogParameters(), r => callbackCalled = true, dialogWindowName);
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();

    _dialogWindowExtendedMock.Raise(dw => dw.Closed += null, EventArgs.Empty);

    callbackCalled.Should().BeTrue();
    _dialogViewModelMock.Verify(vm => vm.OnDialogOpened(It.IsAny<IDialogParameters>()), Times.Once);
    _dialogWindowExtendedMock.Verify(dw => dw.Activate(), Times.Never);
    _user32DllMock.Verify(u => u.ShowWindow(It.IsAny<nint>(), It.IsAny<ShowWindowCommand>()), Times.Never);
  }


  [Theory]
  [MemberData(nameof(ShowAndShowDialogData), true)]
  [MemberData(nameof(ShowAndShowDialogData), false)]
  internal void ShowAndShowDialog_SingleInstanceOpen_SpecialBehaviorApplied(
    Action<ExtendedDialogService, string, IDialogParameters, Action<IDialogResult>, string?> show,
    string? dialogWindowName,
    WindowState windowState,
    bool windowIsActive
  )
  {
    var firstCallbackCalled = false;
    var secondCallbackCalled = false;
    var dialogName = SetupDialog(dialogWindowName, true, windowState, windowIsActive);
    var firstCallDialogParameters = new DialogParameters();
    var secondCallDialogParameters = new DialogParameters();
    var dialogWindowHandle = (nint) 5;
    _dialogWindowExtendedMock.Setup(dw => dw.Handle).Returns(dialogWindowHandle);

    var thread = new Thread(() =>
    {
      var dialogContent = new FrameworkElement
      {
        DataContext = _dialogViewModelMock.Object
      };
      _containerExtensionMock
        .Setup(ce => ce.Resolve(typeof(object), dialogName))
        .Returns(dialogContent);
      show(_testeeService, dialogName, firstCallDialogParameters, r => firstCallbackCalled = true, dialogWindowName);
      show(_testeeService, dialogName, secondCallDialogParameters, r => secondCallbackCalled = true, dialogWindowName);
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();

    _dialogWindowExtendedMock.Raise(dw => dw.Closed += null, EventArgs.Empty);

    firstCallbackCalled.Should().BeTrue();
    secondCallbackCalled.Should().BeFalse();
    _dialogViewModelMock.Verify(
      vm => vm.OnDialogOpened(It.Is<IDialogParameters>(dp => dp == firstCallDialogParameters)),
      Times.Once
    );
    _dialogViewModelMock.Verify(
      vm => vm.OnDialogOpened(It.Is<IDialogParameters>(dp => dp == secondCallDialogParameters)),
      Times.Once
    );
    if (windowState == WindowState.Minimized)
    {
      _user32DllMock.Verify(u => u.ShowWindow(dialogWindowHandle, ShowWindowCommand.SW_RESTORE), Times.Once);
      _dialogWindowExtendedMock.Verify(dw => dw.Activate(), Times.Never);
    }
    else if (!windowIsActive)
    {
      _user32DllMock.VerifyNoOtherCalls();
      _dialogWindowExtendedMock.Verify(dw => dw.Activate(), Times.Once);
    }
    else
    {
      _user32DllMock.VerifyNoOtherCalls();
      _dialogWindowExtendedMock.Verify(dw => dw.Activate(), Times.Never);
    }
  }


  private string SetupDialog(string? dialogWindowName,
                             bool isSingleInstance,
                             WindowState windowState,
                             bool windowIsActive)
  {
    var dialogName = Guid.NewGuid().ToString();
    if (isSingleInstance)
    {
      SingleInstanceDialogsProvider.RegisterSingleInstanceDialog(dialogName);
    }
    if (string.IsNullOrEmpty(dialogWindowName))
    {
      _containerExtensionMock
        .Setup(ce => ce.Resolve(typeof(IDialogWindow), WindowNames.SimpleDialogWindow))
        .Returns(_dialogWindowExtendedMock.Object);
    }
    else
    {
      _containerExtensionMock
        .Setup(ce => ce.Resolve(typeof(IDialogWindow), dialogWindowName))
        .Returns(_dialogWindowExtendedMock.Object);
    }
    _dialogWindowExtendedMock.Setup(dw => dw.DataContext).Returns(_dialogViewModelMock.Object);
    _dialogWindowExtendedMock.Setup(dw => dw.WindowState).Returns(windowState);
    _dialogWindowExtendedMock.Setup(dw => dw.IsActive).Returns(windowIsActive);

    return dialogName;
  }
}
