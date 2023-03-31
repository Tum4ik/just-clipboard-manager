using System.Globalization;
using Moq.Protected;
using Tum4ik.JustClipboardManager.Icons;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.UnitTests.Services.Translation;
public class TranslationServiceTests
{
  private readonly Mock<ISettingsService> _settingsServiceMock = new();

  [Fact]
  internal void SupportedLanguages_HasAtLeastALanguage()
  {
    var testee = new TranslationServiceTestee(_settingsServiceMock.Object, (name, culture) => null);
    testee.SupportedLanguages.Should().NotBeEmpty();
  }


  public static IEnumerable<object[]> SupportedLanguagesData()
  {
    var service = new TranslationServiceTestee(Mock.Of<ISettingsService>(), (name, culture) => null);
    return service.SupportedLanguages.Select(l => new object[] { l });
  }

  [Theory, MemberData(nameof(SupportedLanguagesData))]
  internal void Constructor_SelectedLanguageInitialized(Language language)
  {
    _settingsServiceMock.Setup(ss => ss.Language).Returns(language.Culture);
    var testee = new TranslationServiceTestee(_settingsServiceMock.Object, (name, culture) => null);
    testee.SelectedLanguage.Culture.Should().Be(language.Culture);
  }


  [Fact]
  internal void OnSelectedLanguageChanged_SavedAndNotified()
  {
    var testee = new TranslationServiceTestee(_settingsServiceMock.Object, (name, culture) => null);
    var expectedCulture = CultureInfo.GetCultureInfo("uk-UA");
    var languageChangedInvoked = false;
    testee.LanguageChanged += () => languageChangedInvoked = true;

    testee.SelectedLanguage = new Language(expectedCulture, SvgIconType.Ukraine);

    _settingsServiceMock.VerifySet(ss => ss.Language = expectedCulture, Times.Once);
    languageChangedInvoked.Should().BeTrue();
  }


  [Fact]
  internal void GetTranslation_TranslationExists_ReturnsTranslation()
  {
    const string ExpectedKey = "ExcellentTranslation";
    var expectedCulture = CultureInfo.GetCultureInfo("uk-UA");
    const string ExpectedTranslation = "My excellent translation";
    var testee = new TranslationServiceTestee(_settingsServiceMock.Object, (key, culture) =>
    {
      key.Should().Be(ExpectedKey);
      culture.Should().Be(expectedCulture);
      return ExpectedTranslation;
    });
    _settingsServiceMock.Setup(ss => ss.Language).Returns(expectedCulture);

    var actualTranslation = testee[ExpectedKey];

    actualTranslation.Should().Be(ExpectedTranslation);
  }


  [Fact]
  internal void GetTranslation_TranslationIsAbsent_ReturnsKey()
  {
    const string Key = "ExcellentTranslation";
    var testee = new TranslationServiceTestee(_settingsServiceMock.Object, (name, culture) => null);

    var actualTranslation = testee[Key];

    actualTranslation.Should().Be(Key);
  }
}


internal class TranslationServiceTestee : TranslationService
{
  private readonly Func<string, CultureInfo?, string?> _getStringMock;

  public TranslationServiceTestee(ISettingsService settingsService,
                                  Func<string, CultureInfo?, string?> getStringMock)
    : base(settingsService)
  {
    _getStringMock = getStringMock;
  }


  public override string? GetString(string name, CultureInfo? culture)
  {
    return _getStringMock(name, culture);
  }
}
