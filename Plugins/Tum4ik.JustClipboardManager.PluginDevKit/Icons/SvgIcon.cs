using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using SharpVectors.Converters;
using Tum4ik.JustClipboardManager.PluginDevKit.Attributes;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Icons;
public abstract class SvgIcon<TSvgIcon> : SvgViewbox
  where TSvgIcon : struct, IConvertible
{
  static SvgIcon()
  {
    static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var thisIcon = (SvgIcon<TSvgIcon>) d;
      UpdateColor(d, thisIcon.Color);
    }
    SourceProperty.OverrideMetadata(typeof(SvgIcon<TSvgIcon>), new FrameworkPropertyMetadata(PropertyChangedCallback));
    UriSourceProperty.OverrideMetadata(typeof(SvgIcon<TSvgIcon>), new FrameworkPropertyMetadata(PropertyChangedCallback));
  }


  protected abstract string IconsFolder { get; }


  public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
    nameof(Icon), typeof(TSvgIcon?), typeof(SvgIcon<TSvgIcon>), new((d, e) =>
    {
      var thisIcon = (SvgIcon<TSvgIcon>) d;
      var iconType = (TSvgIcon?) e.NewValue;
      if (!iconType.HasValue)
      {
        return;
      }

      var source = GetSvgSource(iconType.Value, thisIcon.IconsFolder);
      if (source is not null)
      {
        thisIcon.Source = source;
        UpdateColor(d, thisIcon.Color);
      }
    })
  );
  public TSvgIcon? Icon
  {
    get => (TSvgIcon?) GetValue(IconProperty);
    set => SetValue(IconProperty, value);
  }


  public static readonly DependencyProperty AttachedIconProperty = DependencyProperty.RegisterAttached(
    "AttachedIcon", typeof(TSvgIcon?), typeof(SvgIcon<TSvgIcon>)
  );
  [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Desired")]
  public static TSvgIcon? GetAttachedIcon(UIElement target)
  {
    return (TSvgIcon?) target?.GetValue(AttachedIconProperty);
  }
  [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Desired")]
  public static void SetAttachedIcon(UIElement target, TSvgIcon? value)
  {
    target?.SetValue(AttachedIconProperty, value);
  }


  public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
    nameof(Color), typeof(Brush), typeof(SvgIcon<TSvgIcon>), new((d, e) =>
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


  private static Uri? GetSvgSource(TSvgIcon iconType, string iconsFolder)
  {
    var type = iconType.GetType();
    var assembly = type.Assembly;
    var fileName = type
      .GetField(iconType.ToString()!)?
      .GetCustomAttribute<SvgIconResourceAttribute>()?
      .SvgIconResourceName;
    if (fileName is null)
    {
      return null;
    }

    return new($"pack://application:,,,/{assembly};component/{iconsFolder}/{fileName}.svg");
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
