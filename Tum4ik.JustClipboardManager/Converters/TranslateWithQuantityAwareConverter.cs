using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Humanizer;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.Converters;

internal class TranslateWithQuantityAwareConverter : IMultiValueConverter
{
  public object? Convert(object[] values, Type? targetType, object? parameter, CultureInfo? culture)
  {
    if (values.Length != 3)
    {
      throw new ArgumentException($"The converter must accept three bindings.");
    }

    var value1 = values[0];
    var value2 = values[1];
    var value3 = values[2];

    if (value1 == DependencyProperty.UnsetValue
        || value2 == DependencyProperty.UnsetValue
        || value3 == DependencyProperty.UnsetValue)
    {
      return null;
    }
    if (value1 is not ITranslationService translationService)
    {
      throw new ArgumentException($"The first binding must be of {nameof(ITranslationService)} type.");
    }
    if (value2 is not int quantity)
    {
      throw new ArgumentException("The second binding must represent quantity of integer type.");
    }
    if (value3 is null)
    {
      return null;
    }
    var key = value3 is Enum @enum ? @enum.Humanize() : value3.ToString();
    if (key is null)
    {
      return null;
    }

    return translationService[key, quantity];
  }


  public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo? culture)
  {
    throw new NotImplementedException();
  }
}
