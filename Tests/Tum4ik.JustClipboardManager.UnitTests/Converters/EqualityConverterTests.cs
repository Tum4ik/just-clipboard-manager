using Tum4ik.JustClipboardManager.Converters;

namespace Tum4ik.JustClipboardManager.UnitTests.Converters;
public class EqualityConverterTests
{
  private readonly EqualityConverter _testeeConverter = new();

  public static IEnumerable<object[]> InvalidBindingsCountData()
  {
    yield return new object[] { Array.Empty<object>() };
    yield return new object[] { new object[] { 1 } };
    yield return new object[] { new object[] { "one", "two", "three" } };
  }


  [Theory, MemberData(nameof(InvalidBindingsCountData))]
  internal void Convert_InvalidValuesCount_ThrowsException(object[] values)
  {
    Func<object> act = () => _testeeConverter.Convert(values, null, null, null);
    act.Should().Throw<ArgumentException>();
  }


  [Theory]
  [InlineData(1, 2)]
  [InlineData(2, 2)]
  [InlineData("one", "one")]
  [InlineData("two", "one")]
  [InlineData(1, "one")]
  [InlineData("two", 2)]
  internal void ConvertTest(object value1, object value2)
  {
    var converterResult = _testeeConverter.Convert(new object[] { value1, value2 }, null, null, null);
    var expectedResult = value1.Equals(value2);
    converterResult.Should().BeEquivalentTo(expectedResult);
  }


  [Fact]
  internal void ConvertBackIsNotUsed()
  {
    Func<object> act = () => _testeeConverter.ConvertBack(new(), Array.Empty<Type>(), null, null);
    act.Should().Throw<NotImplementedException>();
  }
}
