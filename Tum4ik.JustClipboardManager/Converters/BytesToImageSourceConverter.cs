using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Tum4ik.JustClipboardManager.Converters;
internal class BytesToImageSourceConverter : IValueConverter
{
  public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    var bytes = (byte[]) value;
    if (bytes.Length > 0)
    {
      using var memoryStream = new MemoryStream((byte[]) value);
      return BitmapFrame.Create(memoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
    }

    return null;
  }


  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
