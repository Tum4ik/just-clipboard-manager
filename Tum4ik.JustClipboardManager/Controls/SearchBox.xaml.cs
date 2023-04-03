namespace Tum4ik.JustClipboardManager.Controls;
/// <summary>
/// Interaction logic for SearchBox.xaml
/// </summary>
public partial class SearchBox
{
  public SearchBox()
  {
    InitializeComponent();
  }


  private void ClearButton_Click(object sender, System.Windows.RoutedEventArgs e)
  {
    Text = string.Empty;
  }
}
