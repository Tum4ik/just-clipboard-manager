using System.Globalization;
using Prism.Events;
using Tum4ik.JustClipboardManager.PluginDevKit.Events;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.UnitTests.Services.Translation;
public class TranslationServiceTests
{
  private readonly ISettingsService _settingsService = Substitute.For<ISettingsService>();
  private readonly IEventAggregator _eventAggregator = Substitute.For<IEventAggregator>();

  [Fact]
  internal void SupportedLanguages_HasAtLeastALanguage()
  {
    var testee = new TranslationService(_settingsService, _eventAggregator);
    testee.SupportedLanguages.Should().NotBeEmpty();
  }


  [Fact]
  internal void GetSelectedLanguage_UnsupportedLanguageInSettings_ReturnsFirstLanguageFromList()
  {
    var testee = new TranslationService(_settingsService, _eventAggregator);
    _settingsService.Language.Returns(CultureInfo.GetCultureInfo("ru-RU"));
    var selectedLanguage = testee.SelectedLanguage;
    selectedLanguage.Should().Be(testee.SupportedLanguages.First());
  }


  public static IEnumerable<object[]> SupportedLanguagesData()
  {
    var service = new TranslationService(Substitute.For<ISettingsService>(), Substitute.For<IEventAggregator>());
    return service.SupportedLanguages.Select(l => new object[] { l });
  }


  [Theory, MemberData(nameof(SupportedLanguagesData))]
  internal void GetSelectedLanguage_SupportedLanguageInSettings_ReturnsCorrespondingLanguage(Language language)
  {
    var testee = new TranslationService(_settingsService, _eventAggregator);
    _settingsService.Language.Returns(language.Culture);
    var selectedLanguage = testee.SelectedLanguage;
    selectedLanguage.Culture.Should().Be(language.Culture);
  }


  [Theory, MemberData(nameof(SupportedLanguagesData))]
  internal void SetSelectedLanguage_SelectedLanguageEqualsToAssigningValue_NoLanguageChangeAction(Language language)
  {
    var testee = new TranslationService(_settingsService, _eventAggregator);
    var languageChangedEvent = Substitute.For<LanguageChangedEvent>();
    _settingsService.Language.Returns(language.Culture);
    _eventAggregator.GetEvent<LanguageChangedEvent>().Returns(languageChangedEvent);
    _ = testee.SelectedLanguage;
    testee.SelectedLanguage = language;
    _settingsService.DidNotReceiveWithAnyArgs().Language = default!;
    languageChangedEvent.ReceivedCalls().Any().Should().BeFalse();
  }


  [Theory, MemberData(nameof(SupportedLanguagesData))]
  internal void SetSelectedLanguage_SelectedLanguageIsNotEqualToAssigningValue_LanguageIsChanged(Language language)
  {
    var testee = new TranslationService(_settingsService, _eventAggregator);
    var languageChangedEvent = Substitute.For<LanguageChangedEvent>();
    _eventAggregator.GetEvent<LanguageChangedEvent>().Returns(languageChangedEvent);
    testee.SelectedLanguage = language;
    _settingsService.Received(1).Language = language.Culture;
    languageChangedEvent.Received(1).Publish();
    testee.SelectedLanguage.Culture.Should().Be(language.Culture);
  }


  [Fact]
  internal void GetTranslation_TranslationExists_ReturnsTranslation()
  {
    const string ExpectedKey = "ExcellentTranslation";
    var expectedCulture = CultureInfo.GetCultureInfo("uk-UA");
    const string ExpectedTranslation = "My excellent translation";
    var testee = new TranslationServiceTestee(_settingsService, _eventAggregator,
      (key, culture) =>
      {
        key.Should().Be(ExpectedKey);
        culture.Should().Be(expectedCulture);
        return ExpectedTranslation;
      }
    );
    _settingsService.Language.Returns(expectedCulture);

    var actualTranslation = testee[ExpectedKey];

    actualTranslation.Should().Be(ExpectedTranslation);
  }


  [Fact]
  internal void GetTranslation_TranslationIsAbsent_ReturnsKey()
  {
    const string Key = "ExcellentTranslation";
    var testee = new TranslationServiceTestee(
      _settingsService, _eventAggregator, (name, culture) => null
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
