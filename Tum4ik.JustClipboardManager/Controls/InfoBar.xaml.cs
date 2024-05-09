using System.Windows;
using System.Windows.Controls;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Services;

namespace Tum4ik.JustClipboardManager.Controls;
/// <summary>
/// Interaction logic for InfoBar.xaml
/// </summary>
public partial class InfoBar
{
  public InfoBar()
  {
    InitializeComponent();
    Unloaded += (s, e) =>
    {
      if (InfoBarSubscriber is not null)
      {
        _closeCallback = null;
        InfoBarSubscriber.InfoReceived -= InfoReceived;
      }
    };
  }


  private TextBlock? _title;
  private TextBlock? _body;
  private WinUiButton? _actionButton;

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    _title = (TextBlock) GetTemplateChild("_title");
    _body = (TextBlock) GetTemplateChild("_body");
    _actionButton = (WinUiButton) GetTemplateChild("_actionButton");
  }


  private Action<InfoBarResult>? _closeCallback;


  private void InfoReceived(InfoBarPayload payload)
  {
    Severity = payload.Severity;
    ActionType = payload.ActionType;
    Title = payload.Title;
    Body = payload.Body;
    ActionText = payload.ActionText;
    _closeCallback = payload.Callback;
    IsOpen = true;
  }


  public static readonly DependencyProperty InfoBarSubscriberProperty = DependencyProperty.Register(
    nameof(InfoBarSubscriber), typeof(IInfoBarSubscriber), typeof(InfoBar), new((s, e) =>
    {
      var thisInfoBar = (InfoBar) s;
      if (e.NewValue is IInfoBarSubscriber infoBarSubscriber)
      {
        infoBarSubscriber.InfoReceived += thisInfoBar.InfoReceived;
      }
    })
  );
  internal IInfoBarSubscriber? InfoBarSubscriber
  {
    get => (IInfoBarSubscriber) GetValue(InfoBarSubscriberProperty);
    set => SetValue(InfoBarSubscriberProperty, value);
  }


  public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
    nameof(Severity), typeof(InfoBarSeverity), typeof(InfoBar)
  );
  public InfoBarSeverity Severity
  {
    get => (InfoBarSeverity) GetValue(SeverityProperty);
    set => SetValue(SeverityProperty, value);
  }


  public static readonly DependencyProperty ActionTypeProperty = DependencyProperty.Register(
    nameof(ActionType), typeof(InfoBarActionType), typeof(InfoBar)
  );
  public InfoBarActionType ActionType
  {
    get => (InfoBarActionType) GetValue(ActionTypeProperty);
    set => SetValue(ActionTypeProperty, value);
  }


  public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
    nameof(Title), typeof(string), typeof(InfoBar)
  );
  public string? Title
  {
    get => (string?) GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }


  public static readonly DependencyProperty BodyProperty = DependencyProperty.Register(
    nameof(Body), typeof(string), typeof(InfoBar)
  );
  public string? Body
  {
    get => (string?) GetValue(BodyProperty);
    set => SetValue(BodyProperty, value);
  }


  public static readonly DependencyProperty ActionTextProperty = DependencyProperty.Register(
    nameof(ActionText), typeof(string), typeof(InfoBar)
  );
  public string? ActionText
  {
    get => (string?) GetValue(ActionTextProperty);
    set => SetValue(ActionTextProperty, value);
  }


  public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
    nameof(IsOpen), typeof(bool), typeof(InfoBar)
  );
  public bool IsOpen
  {
    get => (bool) GetValue(IsOpenProperty);
    set => SetValue(IsOpenProperty, value);
  }


  private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
  {
    DefineLayout(e.NewSize.Height);
  }


  private void DefineLayout(double height)
  {
    if (_title is null || _body is null || _actionButton is null)
    {
      return;
    }

    var bodyMargin = _body.Margin;
    var actionButtonMargin = _actionButton.Margin;
    if (height > 50)
    {
      DockPanel.SetDock(_title, Dock.Top);
      DockPanel.SetDock(_actionButton, Dock.Bottom);
      bodyMargin.Left = 12;
      bodyMargin.Top = 1;
      actionButtonMargin.Left = 12;
      actionButtonMargin.Bottom = 12;
      _actionButton.HorizontalAlignment = HorizontalAlignment.Left;
    }
    else
    {
      DockPanel.SetDock(_title, Dock.Left);
      DockPanel.SetDock(_actionButton, Dock.Right);
      bodyMargin.Left = 0;
      bodyMargin.Top = 14;
      actionButtonMargin.Left = 0;
      actionButtonMargin.Bottom = 0;
      _actionButton.HorizontalAlignment = HorizontalAlignment.Right;
    }

    _body.Margin = bodyMargin;
    _actionButton.Margin = actionButtonMargin;
  }


  private void CloseButton_Click(object sender, RoutedEventArgs e)
  {
    Close(InfoBarResult.Cancel);
  }

  private void ActionButton_Click(object sender, RoutedEventArgs e)
  {
    Close(InfoBarResult.Action);
  }

  private void Close(InfoBarResult result)
  {
    IsOpen = false;
    _closeCallback?.Invoke(result);
  }

  private static readonly DependencyProperty CleanUpStartedProperty = DependencyProperty.Register(
    nameof(CleanUpStarted), typeof(bool), typeof(InfoBar), new((s, e) =>
    {
      if (e.NewValue is true)
      {
        var thisInfoBar = (InfoBar) s;

        thisInfoBar.Title = null;
        thisInfoBar.Body = null;
        thisInfoBar.ActionText = null;
        thisInfoBar.DefineLayout(50);
      }
    })
  );
  private bool CleanUpStarted
  {
    get => (bool) GetValue(CleanUpStartedProperty);
    set => SetValue(CleanUpStartedProperty, value);
  }
}
