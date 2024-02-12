namespace Tum4ik.JustClipboardManager.Controls;

internal sealed partial class AppBarNavigationViewItem
{
  public AppBarNavigationViewItem()
  {
    InitializeComponent();
  }


  public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(
    nameof(Glyph), typeof(string), typeof(AppBarNavigationViewItem), new(null)
  );
  public string? Glyph
  {
    get => (string) GetValue(GlyphProperty);
    set => SetValue(GlyphProperty, value);
  }


  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(AppBarNavigationViewItem), new(null)
  );
  public string? Text
  {
    get => (string) GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }
}
