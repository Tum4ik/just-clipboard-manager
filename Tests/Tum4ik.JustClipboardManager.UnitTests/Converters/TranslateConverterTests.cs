using System.Windows;
using Tum4ik.JustClipboardManager.Converters;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.UnitTests.Converters;
public class TranslateConverterTests
{
  private readonly ITranslationService _translationService = Substitute.For<ITranslationService>();
  private readonly TranslateConverter _testeeConverter = new();

  public static IEnumerable<object[]> InvalidBindingsCountData()
  {
    yield return new object[] { Array.Empty<object>() };
    yield return new object[] { new object[] { 1 } };
    yield return new object[] { new object[] { "one", "two", "three" } };
  }


  [Theory, MemberData(nameof(InvalidBindingsCountData))]
  internal void Convert_InvalidValuesCount_ThrowsException(object[] values)
  {
    Func<object?> act = () => _testeeConverter.Convert(values, null, null, null);
    act.Should().Throw<ArgumentException>();
  }


  [Fact]
  internal void Convert_UnsetFirstValue_ReturnsNull()
  {
    var result = _testeeConverter.Convert(new object[] { DependencyProperty.UnsetValue, "any" }, null, null, null);
    result.Should().BeNull();
  }


  [Fact]
  internal void Convert_UnsetSecondValue_ReturnsNull()
  {
    var result = _testeeConverter.Convert(new object[] { "any", DependencyProperty.UnsetValue }, null, null, null);
    result.Should().BeNull();
  }


  [Fact]
  internal void Convert_FirstValueIsNotTranslationService_ThrowsException()
  {
    Func<object?> act = () => _testeeConverter.Convert(new object[] {"any", "any"}, null, null, null);
    act.Should().Throw<ArgumentException>();
  }


  [Fact]
  internal void Convert_FirstValueIsTranslationServiceSecondValueIsNull_ReturnsNull()
  {
    var result = _testeeConverter.Convert(new object[] { _translationService, null }, null, null, null);
    result.Should().BeNull();
  }


  [Fact]
  internal void Convert_FirstValueIsTranslationServiceSecondValueIsNotString_ThrowsException()
  {
    Func<object?> act = () => _testeeConverter.Convert(new object[] { _translationService, 3 }, null, null, null);
    act.Should().Throw<ArgumentException>();
  }


  [Fact]
  internal void Convert_FirstValueIsTranslationServiceSecondValueIsString_ReturnsTranslation()
  {
    const string Key = "key";
    const string ExpectedTranslation = "ключ";
    _translationService[Key].Returns(ExpectedTranslation);
    var result = _testeeConverter.Convert(new object[] {_translationService, Key}, null, null, null);
    result.Should().BeEquivalentTo(ExpectedTranslation);
  }


  [Fact]
  internal void ConvertBackIsNotUsed()
  {
    Func<object> act = () => _testeeConverter.ConvertBack(new(), Array.Empty<Type>(), null, null);
    act.Should().Throw<NotImplementedException>();
  }
}
