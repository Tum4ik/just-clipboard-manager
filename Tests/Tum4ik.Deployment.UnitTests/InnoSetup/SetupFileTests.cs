using System.Text;

namespace Tum4ik.Deployment.UnitTests.InnoSetup;
public class SetupFileTests
{
  [Fact]
  public void SetupFile_MustHaveBom()
  {
    var filepath = "..\\..\\..\\..\\..\\InnoSetup\\Setup.iss";
    using var reader = new StreamReader(filepath, Encoding.Default, true);
    if (reader.Peek() >= 0)
    {
      reader.Read();
    }

    var preamble = reader.CurrentEncoding.Preamble;

    preamble.Length.Should().Be(3);
    preamble[0].Should().Be(239);
    preamble[1].Should().Be(187);
    preamble[2].Should().Be(191);
  }
}
