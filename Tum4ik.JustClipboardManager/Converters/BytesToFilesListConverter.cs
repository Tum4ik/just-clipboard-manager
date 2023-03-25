using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Tum4ik.JustClipboardManager.Converters;
internal class BytesToFilesListConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    var bytes = (byte[]) value;
    var str = Encoding.UTF8.GetString(bytes);
    return str.Replace(";", Environment.NewLine, StringComparison.OrdinalIgnoreCase);
  }


  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
