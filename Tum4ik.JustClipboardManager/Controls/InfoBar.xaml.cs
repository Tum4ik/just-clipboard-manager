using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Tum4ik.JustClipboardManager.Controls;
/// <summary>
/// Interaction logic for InfoBar.xaml
/// </summary>
public partial class InfoBar
{
  public InfoBar()
  {
    InitializeComponent();
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


  public static readonly DependencyProperty ActionCommandProperty = DependencyProperty.Register(
    nameof(ActionCommand), typeof(ICommand), typeof(InfoBar)
  );
  public ICommand? ActionCommand
  {
    get => (ICommand?) GetValue(ActionCommandProperty);
    set => SetValue(ActionCommandProperty, value);
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


  private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
  {
    var title = (TextBlock) GetTemplateChild("_title");
    var body = (TextBlock) GetTemplateChild("_body");
    var actionButton = (Button) GetTemplateChild("_actionButton");
    var bodyMargin = body.Margin;
    var actionButtonMargin = actionButton.Margin;
    if (e.NewSize.Height > 50)
    {
      DockPanel.SetDock(title, Dock.Top);
      DockPanel.SetDock(actionButton, Dock.Bottom);
      bodyMargin.Left = 12;
      bodyMargin.Top = 1;
      actionButtonMargin.Left = 12;
      actionButtonMargin.Bottom = 12;
      actionButton.HorizontalAlignment = HorizontalAlignment.Left;
    }
    else
    {
      DockPanel.SetDock(title, Dock.Left);
      DockPanel.SetDock(actionButton, Dock.Right);
      bodyMargin.Left = 0;
      bodyMargin.Top = 14;
      actionButtonMargin.Left = 0;
      actionButtonMargin.Bottom = 0;
      actionButton.HorizontalAlignment = HorizontalAlignment.Right;
    }

    body.Margin = bodyMargin;
    actionButton.Margin = actionButtonMargin;
  }
}


public enum InfoBarSeverity
{
  Informational, Success, Warning, Critical
}

public enum InfoBarActionType
{
  None, /*HyperlinkButton,*/ Button
}
