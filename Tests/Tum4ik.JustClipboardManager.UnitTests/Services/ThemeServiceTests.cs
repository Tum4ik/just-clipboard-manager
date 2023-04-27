using System.Windows;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Theme;

namespace Tum4ik.JustClipboardManager.UnitTests.Services;

[Collection(TestCollections.ApplicationInstanceRequired)]
public class ThemeServiceTests
{
  private readonly Mock<ISettingsService> _settingsServiceMock = new();
  private readonly Mock<IEventAggregator> _eventAggregatorMock = new();
  private readonly Mock<IAppResourcesService> _appResourcesServiceMock = new();
  private readonly ResourceDictionary _appResources = new();
  private readonly Mock<ThemeChangedEvent> _themeChangedEventMock = new();


  private ThemeService CreateTesteeService()
  {
    _appResourcesServiceMock.Setup(r => r.Resources).Returns(_appResources);
    _eventAggregatorMock.Setup(ea => ea.GetEvent<ThemeChangedEvent>()).Returns(_themeChangedEventMock.Object);
    return new(
      _settingsServiceMock.Object,
      _eventAggregatorMock.Object,
      _appResourcesServiceMock.Object
    );
  }


  [Fact]
  internal void SetThemeOnInitializationTest()
  {
    _ = CreateTesteeService();
    _themeChangedEventMock.Verify(e => e.Publish(), Times.Once);
  }


  [Fact]
  internal void Themes_HasAtLeastOne()
  {
    var testeeService = CreateTesteeService();
    testeeService.Themes.Should().NotBeEmpty();
  }


  [Fact]
  internal void GetSelectedTheme_NoThemeInSettings_ReturnsFirstThemeFromList()
  {
    _settingsServiceMock.Setup(ss => ss.Theme).Returns(string.Empty);
    var testeeService = CreateTesteeService();
    var selectedTheme = testeeService.SelectedTheme;
    selectedTheme.Should().Be(testeeService.Themes.First());
  }


  [Theory]
  [InlineData("Light")]
  [InlineData("Dark")]
  internal void GetSelectedTheme_ThemeIsInSettings_ReturnsCorrespondingTheme(string themeName)
  {
    _settingsServiceMock.Setup(ss => ss.Theme).Returns(themeName);
    var testeeService = CreateTesteeService();
    var selectedTheme = testeeService.SelectedTheme;
    selectedTheme.Name.Should().Be(themeName);
  }


  [Theory]
  [InlineData("Light")]
  [InlineData("Dark")]
  internal void SetSelectedTheme_SelectedThemeEqualsToAssigningValue_NoThemeChangeAction(string themeName)
  {
    _settingsServiceMock.Setup(ss => ss.Theme).Returns(themeName);
    var testeeService = CreateTesteeService();
    _settingsServiceMock.VerifySet(ss => ss.Theme = themeName, Times.Once);
    _themeChangedEventMock.Verify(e => e.Publish(), Times.Once);
    _settingsServiceMock.Reset();
    _themeChangedEventMock.Reset();
    testeeService.SelectedTheme = testeeService.Themes.First(ct => ct.Name == themeName);
    _settingsServiceMock.VerifyNoOtherCalls();
    _themeChangedEventMock.VerifyNoOtherCalls();
    testeeService.SelectedTheme.Name.Should().Be(themeName);
  }


  [Theory]
  [InlineData("Light")]
  [InlineData("Dark")]
  internal void SetSelectedTheme_SelectedThemeIsNotEqualToAssigningValue_ThemeIsChanged(string themeName)
  {
    _settingsServiceMock.Setup(ss => ss.Theme).Returns(themeName);
    var testeeService = CreateTesteeService();
    _settingsServiceMock.Reset();
    _themeChangedEventMock.Reset();
    var newTheme = testeeService.Themes.First(ct => ct.Name != themeName);
    testeeService.SelectedTheme = newTheme;
    _appResources.MergedDictionaries.Should().Contain(rd => rd.Source.ToString().EndsWith(newTheme.XamlFileName));
    _settingsServiceMock.VerifySet(ss => ss.Theme = newTheme.Name, Times.Once);
    _themeChangedEventMock.Verify(e => e.Publish(), Times.Once);
    testeeService.SelectedTheme.Should().Be(newTheme);
  }
}
