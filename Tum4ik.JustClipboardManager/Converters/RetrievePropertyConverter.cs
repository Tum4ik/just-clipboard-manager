using System.Globalization;
using System.Windows.Data;

namespace Tum4ik.JustClipboardManager.Converters;

internal class RetrievePropertyConverter : IMultiValueConverter
{
  public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    if (values.Length != 2)
    {
      throw new ArgumentException(
        $"The {nameof(RetrievePropertyConverter)} must consume two bindings, but {values.Length} is given."
      );
    }

    var selectedValueObj = values[0];
    var selectedValuePathObj = values[1];

    if (selectedValueObj is null
        || selectedValuePathObj is not string propertyName
        || string.IsNullOrWhiteSpace(propertyName))
    {
      return selectedValueObj;
    }

    var propInfo = selectedValueObj.GetType().GetProperty(propertyName);
    if (propInfo is null)
    {
      return selectedValueObj;
    }

    return propInfo.GetValue(selectedValueObj);
  }


  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
