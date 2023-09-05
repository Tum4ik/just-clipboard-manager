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
  private readonly IContainerExtension _containerExtension = Substitute.For<IContainerExtension>();
  private readonly IUser32DllService _user32Dll = Substitute.For<IUser32DllService>();
  private readonly IDialogWindowExtended _dialogWindowExtended = Substitute.For<IDialogWindowExtended>();
  private readonly IDialogAware _dialogViewModel = Substitute.For<IDialogAware>();
  private readonly ExtendedDialogService _testeeService;

  public ExtendedDialogServiceTests()
  {
    _testeeService = new(_containerExtension, _user32Dll);
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
        DataContext = _dialogViewModel
      };
      _containerExtension
        .Resolve(typeof(object), dialogName)
        .Returns(dialogContent);
      show(_testeeService, dialogName, new DialogParameters(), r => callbackCalled = true, dialogWindowName);
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();

    _dialogWindowExtended.Closed += Raise.Event();

    callbackCalled.Should().BeTrue();
    _dialogViewModel.ReceivedWithAnyArgs(1).OnDialogOpened(default);
    _dialogWindowExtended.DidNotReceive().Activate();
    _user32Dll.DidNotReceiveWithAnyArgs().ShowWindow(default, default);
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
        DataContext = _dialogViewModel
      };
      _containerExtension
        .Resolve(typeof(object), dialogName)
        .Returns(dialogContent);
      show(_testeeService, dialogName, new DialogParameters(), r => callbackCalled = true, dialogWindowName);
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();

    _dialogWindowExtended.Closed += Raise.Event();

    callbackCalled.Should().BeTrue();
    _dialogViewModel.ReceivedWithAnyArgs(1).OnDialogOpened(default);
    _dialogWindowExtended.DidNotReceive().Activate();
    _user32Dll.DidNotReceiveWithAnyArgs().ShowWindow(default, default);
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
    _dialogWindowExtended.Handle.Returns(dialogWindowHandle);

    var thread = new Thread(() =>
    {
      var dialogContent = new FrameworkElement
      {
        DataContext = _dialogViewModel
      };
      _containerExtension
        .Resolve(typeof(object), dialogName)
        .Returns(dialogContent);
      show(_testeeService, dialogName, firstCallDialogParameters, r => firstCallbackCalled = true, dialogWindowName);
      show(_testeeService, dialogName, secondCallDialogParameters, r => secondCallbackCalled = true, dialogWindowName);
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();

    _dialogWindowExtended.Closed += Raise.Event();

    firstCallbackCalled.Should().BeTrue();
    secondCallbackCalled.Should().BeFalse();
    _dialogViewModel.Received(1).OnDialogOpened(firstCallDialogParameters);
    _dialogViewModel.Received(1).OnDialogOpened(secondCallDialogParameters);
    if (windowState == WindowState.Minimized)
    {
      _user32Dll.Received(1).ShowWindow(dialogWindowHandle, ShowWindowCommand.SW_RESTORE);
      _dialogWindowExtended.DidNotReceive().Activate();
    }
    else if (!windowIsActive)
    {
      _user32Dll.ReceivedCalls().Any().Should().BeFalse();
      _dialogWindowExtended.Received(1).Activate();
    }
    else
    {
      _user32Dll.ReceivedCalls().Any().Should().BeFalse();
      _dialogWindowExtended.DidNotReceive().Activate();
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
      _containerExtension
        .Resolve(typeof(IDialogWindow), WindowNames.SimpleDialogWindow)
        .Returns(_dialogWindowExtended);
    }
    else
    {
      _containerExtension
        .Resolve(typeof(IDialogWindow), dialogWindowName)
        .Returns(_dialogWindowExtended);
    }
    _dialogWindowExtended.DataContext.Returns(_dialogViewModel);
    _dialogWindowExtended.WindowState.Returns(windowState);
    _dialogWindowExtended.IsActive.Returns(windowIsActive);

    return dialogName;
  }
}
