using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tum4ik.JustClipboardManager.Converters;

internal class SizeToRectConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    if (values.Length != 2)
    {
      throw new ArgumentException($"{nameof(SizeToRectConverter)} must accept two bindings: width and height.");
    }
    if (values[0] is not double width || values[1] is not double height)
    {
      throw new ArgumentException($"The arguments must be of double type.");
    }
    return new Rect(0, 0, width, height);
  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
