using System.Globalization;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.UnitTests.Services.Translation;
public class TranslationServiceTests
{
  private readonly Mock<ISettingsService> _settingsServiceMock = new();
  private readonly Mock<IEventAggregator> _eventAggregatorMock = new();

  [Fact]
  internal void SupportedLanguages_HasAtLeastALanguage()
  {
    var testee = new TranslationService(_settingsServiceMock.Object, _eventAggregatorMock.Object);
    testee.SupportedLanguages.Should().NotBeEmpty();
  }


  [Fact]
  internal void GetSelectedLanguage_UnsupportedLanguageInSettings_ReturnsFirstLanguageFromList()
  {
    var testee = new TranslationService(_settingsServiceMock.Object, _eventAggregatorMock.Object);
    _settingsServiceMock.Setup(ss => ss.Language).Returns(CultureInfo.GetCultureInfo("ru-RU"));
    var selectedLanguage = testee.SelectedLanguage;
    selectedLanguage.Should().Be(testee.SupportedLanguages.First());
  }


  public static IEnumerable<object[]> SupportedLanguagesData()
  {
    var service = new TranslationService(Mock.Of<ISettingsService>(), Mock.Of<IEventAggregator>());
    return service.SupportedLanguages.Select(l => new object[] { l });
  }


  [Theory, MemberData(nameof(SupportedLanguagesData))]
  internal void GetSelectedLanguage_SupportedLanguageInSettings_ReturnsCorrespondingLanguage(Language language)
  {
    var testee = new TranslationService(_settingsServiceMock.Object, _eventAggregatorMock.Object);
    _settingsServiceMock.Setup(ss => ss.Language).Returns(language.Culture);
    var selectedLanguage = testee.SelectedLanguage;
    selectedLanguage.Culture.Should().Be(language.Culture);
  }


  [Theory, MemberData(nameof(SupportedLanguagesData))]
  internal void SetSelectedLanguage_SelectedLanguageEqualsToAssigningValue_NoLanguageChangeAction(Language language)
  {
    var testee = new TranslationService(_settingsServiceMock.Object, _eventAggregatorMock.Object);
    var languageChangedEventMock = new Mock<LanguageChangedEvent>();
    _settingsServiceMock.Setup(ss => ss.Language).Returns(language.Culture);
    _eventAggregatorMock.Setup(ea => ea.GetEvent<LanguageChangedEvent>()).Returns(languageChangedEventMock.Object);
    _ = testee.SelectedLanguage;
    testee.SelectedLanguage = language;
    _settingsServiceMock.VerifySet(ss => ss.Language = It.IsAny<CultureInfo>(), Times.Never);
    languageChangedEventMock.VerifyNoOtherCalls();
  }


  [Theory, MemberData(nameof(SupportedLanguagesData))]
  internal void SetSelectedLanguage_SelectedLanguageIsNotEqualToAssigningValue_LanguageIsChanged(Language language)
  {
    var testee = new TranslationService(_settingsServiceMock.Object, _eventAggregatorMock.Object);
    var languageChangedEventMock = new Mock<LanguageChangedEvent>();
    _eventAggregatorMock.Setup(ea => ea.GetEvent<LanguageChangedEvent>()).Returns(languageChangedEventMock.Object);
    testee.SelectedLanguage = language;
    _settingsServiceMock.VerifySet(ss => ss.Language = language.Culture, Times.Once);
    languageChangedEventMock.Verify(e => e.Publish(), Times.Once);
    testee.SelectedLanguage.Culture.Should().Be(language.Culture);
  }


  [Fact]
  internal void GetTranslation_TranslationExists_ReturnsTranslation()
  {
    const string ExpectedKey = "ExcellentTranslation";
    var expectedCulture = CultureInfo.GetCultureInfo("uk-UA");
    const string ExpectedTranslation = "My excellent translation";
    var testee = new TranslationServiceTestee(_settingsServiceMock.Object, _eventAggregatorMock.Object,
      (key, culture) =>
      {
        key.Should().Be(ExpectedKey);
        culture.Should().Be(expectedCulture);
        return ExpectedTranslation;
      }
    );
    _settingsServiceMock.Setup(ss => ss.Language).Returns(expectedCulture);

    var actualTranslation = testee[ExpectedKey];

    actualTranslation.Should().Be(ExpectedTranslation);
  }


  [Fact]
  internal void GetTranslation_TranslationIsAbsent_ReturnsKey()
  {
    const string Key = "ExcellentTranslation";
    var testee = new TranslationServiceTestee(
      _settingsServiceMock.Object, _eventAggregatorMock.Object, (name, culture) => null
    );

    var actualTranslation = testee[Key];

    actualTranslation.Should().Be(Key);
  }
}


internal class TranslationServiceTestee : TranslationService
{
  private readonly Func<string, CultureInfo?, string?> _getStringMock;

  public TranslationServiceTestee(ISettingsService settingsService,
                                  IEventAggregator eventAggregator,
                                  Func<string, CultureInfo?, string?> getStringMock)
    : base(settingsService, eventAggregator)
  {
    _getStringMock = getStringMock;
  }


  public override string? GetString(string name, CultureInfo? culture)
  {
    return _getStringMock(name, culture);
  }
}
