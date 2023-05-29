using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using SharpVectors.Converters;

namespace Tum4ik.JustClipboardManager.Icons;
public class SvgIcon : SvgViewbox
{
  static SvgIcon()
  {
    static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var thisIcon = (SvgIcon) d;
      UpdateColor(d, thisIcon.Color);
    }
    SourceProperty.OverrideMetadata(typeof(SvgIcon), new FrameworkPropertyMetadata(PropertyChangedCallback));
    UriSourceProperty.OverrideMetadata(typeof(SvgIcon), new FrameworkPropertyMetadata(PropertyChangedCallback));
  }


  public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
    nameof(Icon), typeof(SvgIconType?), typeof(SvgIcon), new((d, e) =>
    {
      var thisIcon = (SvgIcon) d;
      var iconType = (SvgIconType?) e.NewValue;
      if (!iconType.HasValue)
      {
        return;
      }

      var source = GetSvgSource(iconType.Value);
      if (source is not null)
      {
        thisIcon.Source = source;
        UpdateColor(d, thisIcon.Color);
      }
    })
  );
  public SvgIconType? Icon
  {
    get => (SvgIconType?) GetValue(IconProperty);
    set => SetValue(IconProperty, value);
  }


  public static readonly DependencyProperty AttachedIconProperty = DependencyProperty.RegisterAttached(
    "AttachedIcon", typeof(SvgIconType?), typeof(SvgIcon)
  );
  public static SvgIconType? GetAttachedIcon(UIElement target)
  {
    return (SvgIconType?) target.GetValue(AttachedIconProperty);
  }
  public static void SetAttachedIcon(UIElement target, SvgIconType? value)
  {
    target.SetValue(AttachedIconProperty, value);
  }


  public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
      nameof(Color), typeof(Brush), typeof(SvgIcon), new((d, e) =>
      {
        var newBrush = (Brush?) e.NewValue;
        UpdateColor(d, newBrush);
      })
  );
  public Brush? Color
  {
    get => (Brush?) GetValue(ColorProperty);
    set => SetValue(ColorProperty, value);
  }


  private static Uri? GetSvgSource(SvgIconType iconType)
  {
    var type = iconType.GetType();
    var assembly = type.Assembly;
    var fileName = type
      .GetField(iconType.ToString())?
      .GetCustomAttribute<SvgIconResourceAttribute>()?
      .SvgIconResourceName;
    if (fileName is null)
    {
      return null;
    }

    return new($"pack://application:,,,/{assembly};component/Resources/{fileName}.svg");
  }


  private static void UpdateColor(DependencyObject d, Brush? brush)
  {
    if (brush is null)
    {
      return;
    }

    var thisIconCanvas = ((SvgViewbox) d).DrawingCanvas;
    foreach (var drawing in thisIconCanvas.DrawObjects)
    {
      ChangeDrawingColor(drawing, brush);
    }
  }


  private static void ChangeDrawingColor(Drawing drawing, Brush brush)
  {
    if (drawing is DrawingGroup drawingGroup)
    {
      foreach (var child in drawingGroup.Children)
      {
        ChangeDrawingColor(child, brush);
      }
    }
    else if (drawing is GeometryDrawing geometryDrawing)
    {
      geometryDrawing.Brush = brush;
    }
  }
}
