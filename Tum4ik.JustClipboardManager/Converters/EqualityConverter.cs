using System.Globalization;
using System.Windows.Data;

namespace Tum4ik.JustClipboardManager.Converters;
internal class EqualityConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    if (values.Length != 2)
    {
      throw new ArgumentException($"{nameof(EqualityConverter)} must accept two bindings.");
    }

    return values[0].Equals(values[1]);
  }


  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
