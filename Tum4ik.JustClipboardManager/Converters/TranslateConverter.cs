using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.Converters;

internal class TranslateConverter : IMultiValueConverter
{
  public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    if (values.Length != 2)
    {
      throw new ArgumentException($"{nameof(TranslateConverter)} must accept two bindings.");
    }

    var value0 = values[0];
    var value1 = values[1];
    if (value0 == DependencyProperty.UnsetValue && value1 == DependencyProperty.UnsetValue)
    {
      return null;
    }

    if (value0 is not ITranslationService translationService)
    {
      throw new ArgumentException($"The first binding must be of {nameof(ITranslationService)} type.");
    }
    if (value1 is null)
    {
      return null;
    }
    if (value1 is not string key)
    {
      throw new ArgumentException($"The second binding must be a string.");
    }

    return translationService[key];
  }


  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
