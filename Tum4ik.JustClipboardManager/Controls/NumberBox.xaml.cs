using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Tum4ik.JustClipboardManager.Controls;

/// <summary>
/// Interaction logic for NumberBox.xaml
/// </summary>
public partial class NumberBox
{
  public NumberBox()
  {
    InitializeComponent();
  }


  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    var window = Window.GetWindow(this);
    var popup = (Popup) GetTemplateChild("_spinnerPopup");
    window.LocationChanged += (s, e) =>
    {
      // to move spinner popup together with the window
      var o = popup.HorizontalOffset;
      popup.HorizontalOffset++;
      popup.HorizontalOffset = o;
    };
  }


  public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
    nameof(MinValue), typeof(int), typeof(NumberBox), new(int.MinValue)
  );
  public int MinValue
  {
    get => (int) GetValue(MinValueProperty);
    set => SetValue(MinValueProperty, value);
  }


  public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
    nameof(MaxValue), typeof(int), typeof(NumberBox), new(int.MaxValue)
  );
  public int MaxValue
  {
    get => (int) GetValue(MaxValueProperty);
    set => SetValue(MaxValueProperty, value);
  }


  public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
    nameof(Step), typeof(int), typeof(NumberBox), new(1)
  );
  public int Step
  {
    get => (int) GetValue(StepProperty);
    set => SetValue(StepProperty, value);
  }


  private string? _valueBeforeClear;

  private void ClearButton_Click(object sender, RoutedEventArgs e)
  {
    _valueBeforeClear = Text;
    Text = string.Empty;
  }


  private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
  {
    var resultText = Text;
    if (SelectionLength > 0)
    {
      resultText = resultText.Remove(SelectionStart, SelectionLength).Insert(SelectionStart, e.Text);
    }
    else
    {
      resultText = resultText.Insert(CaretIndex, e.Text);
    }

    e.Handled = !(int.TryParse(resultText, out _));
  }


  private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
  {
    e.Handled = e.Key == Key.Space;
  }


  private void TextBox_GotFocus(object sender, RoutedEventArgs e)
  {
    SelectAll();
  }


  private void TextBox_LostFocus(object sender, RoutedEventArgs e)
  {
    if (string.IsNullOrEmpty(Text))
    {
      Text = _valueBeforeClear;
    }
  }


  private void IncreaseButton_Click(object sender, RoutedEventArgs e)
  {
    DoStep(Step);
  }


  private void DecreaseButton_Click(object sender, RoutedEventArgs e)
  {
    DoStep(-Step);
  }


  private void DoStep(int step)
  {
    if (int.TryParse(Text, out var number))
    {
      var result = number + step;
      if (result >= MinValue && result <= MaxValue)
      {
        Text = result.ToString(CultureInfo.InvariantCulture);
        SelectAll();
      }
    }
  }
}
