using System.Text;

namespace Tum4ik.DeploymentTests.InnoSetup;
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

    Assert.Equal(3, preamble.Length);
    Assert.Equal(239, preamble[0]);
    Assert.Equal(187, preamble[1]);
    Assert.Equal(191, preamble[2]);
  }
}
