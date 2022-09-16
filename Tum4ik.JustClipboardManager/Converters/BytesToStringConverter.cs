using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Tum4ik.JustClipboardManager.Converters;
internal class BytesToStringConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return Encoding.UTF8.GetString((byte[]) value);
  }


  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
