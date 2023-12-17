using System.Windows;
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


  private void ClearButton_Click(object sender, RoutedEventArgs e)
  {
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

    e.Handled = !(int.TryParse(resultText, out var value) && value >= MinValue && value <= MaxValue);
  }


  private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
  {
    e.Handled = e.Key == Key.Space;
  }


  private void TextBox_GotFocus(object sender, RoutedEventArgs e)
  {
    SelectAll();
  }
}
