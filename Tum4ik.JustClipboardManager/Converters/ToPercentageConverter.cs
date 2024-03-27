using System.Globalization;
using System.Windows.Data;

namespace Tum4ik.JustClipboardManager.Converters;

internal class ToPercentageConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is not double number)
    {
      throw new ArgumentException("The value must be double.", nameof(value));
    }
    if (number < 0)
    {
      return "0%";
    }
    if (number > 1)
    {
      return "100%";
    }
    return $"{(int) (number * 100)}%";
  }


  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
