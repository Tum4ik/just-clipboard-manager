using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Tum4ik.JustClipboardManager.Controls;
/// <summary>
/// Interaction logic for ModernComboBox.xaml
/// </summary>
public partial class ModernComboBox
{
  public ModernComboBox()
  {
    InitializeComponent();
  }


  public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register(
    nameof(SelectedItemTemplate), typeof(DataTemplate), typeof(ModernComboBox), new(DefaultSelectedItemTemplate())
  );
  public DataTemplate SelectedItemTemplate
  {
    get => (DataTemplate) GetValue(SelectedItemTemplateProperty);
    set => SetValue(SelectedItemTemplateProperty, value);
  }


  protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
  {
    base.OnPreviewMouseDown(e);
    Focusable = false;
  }


  protected override void OnDropDownClosed(EventArgs e)
  {
    base.OnDropDownClosed(e);
    Focusable = true;
  }


  private void RadioButton_Click(object sender, RoutedEventArgs e)
  {
    IsDropDownOpen = false;
  }


  private static DataTemplate DefaultSelectedItemTemplate()
  {
    var visualTree = new FrameworkElementFactory(typeof(TextBlock));
    visualTree.SetBinding(TextBlock.TextProperty, new Binding());
    visualTree.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
    visualTree.SetValue(TextBlock.MarginProperty, new Thickness(12, 0, 0, 0));
    return new DataTemplate
    {
      DataType = typeof(string),
      VisualTree = visualTree
    };
  }
}
