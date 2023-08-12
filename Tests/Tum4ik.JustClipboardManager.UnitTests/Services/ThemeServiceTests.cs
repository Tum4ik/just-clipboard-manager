using System.Windows;
using Prism.Events;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Theme;

namespace Tum4ik.JustClipboardManager.UnitTests.Services;

[Collection(TestCollections.ApplicationInstanceRequired)]
public class ThemeServiceTests
{
  private readonly ISettingsService _settingsService = Substitute.For<ISettingsService>();
  private readonly IEventAggregator _eventAggregator = Substitute.For<IEventAggregator>();
  private readonly IAppResourcesService _appResourcesService = Substitute.For<IAppResourcesService>();
  private readonly ResourceDictionary _appResources = new();
  private readonly ThemeChangedEvent _themeChangedEvent = Substitute.For<ThemeChangedEvent>();


  private ThemeService CreateTesteeService()
  {
    _appResourcesService.Resources.Returns(_appResources);
    _eventAggregator.GetEvent<ThemeChangedEvent>().Returns(_themeChangedEvent);
    return new(
      _settingsService,
      _eventAggregator,
      _appResourcesService
    );
  }


  [Fact]
  internal void SetThemeOnInitializationTest()
  {
    _ = CreateTesteeService();
    _themeChangedEvent.Received(1).Publish();
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
    _settingsService.Theme.Returns(string.Empty);
    var testeeService = CreateTesteeService();
    var selectedTheme = testeeService.SelectedTheme;
    selectedTheme.Should().Be(testeeService.Themes.First());
  }


  [Theory]
  [InlineData("Light")]
  [InlineData("Dark")]
  internal void GetSelectedTheme_ThemeIsInSettings_ReturnsCorrespondingTheme(string themeName)
  {
    _settingsService.Theme.Returns(themeName);
    var testeeService = CreateTesteeService();
    var selectedTheme = testeeService.SelectedTheme;
    selectedTheme.Name.Should().Be(themeName);
  }


  [Theory]
  [InlineData("Light")]
  [InlineData("Dark")]
  internal void SetSelectedTheme_SelectedThemeEqualsToAssigningValue_NoThemeChangeAction(string themeName)
  {
    _settingsService.Theme.Returns(themeName);
    var testeeService = CreateTesteeService();
    _settingsService.Received(1).Theme = themeName;
    _themeChangedEvent.Received(1).Publish();
    _settingsService.ClearReceivedCalls();
    _themeChangedEvent.ClearReceivedCalls();
    testeeService.SelectedTheme = testeeService.Themes.First(ct => ct.Name == themeName);
    _settingsService.ReceivedCalls().Any().Should().BeFalse();
    _themeChangedEvent.ReceivedCalls().Any().Should().BeFalse();
    testeeService.SelectedTheme.Name.Should().Be(themeName);
  }


  [Theory]
  [InlineData("Light")]
  [InlineData("Dark")]
  internal void SetSelectedTheme_SelectedThemeIsNotEqualToAssigningValue_ThemeIsChanged(string themeName)
  {
    _settingsService.Theme.Returns(themeName);
    var testeeService = CreateTesteeService();
    _settingsService.ClearReceivedCalls();
    _themeChangedEvent.ClearReceivedCalls();
    var newTheme = testeeService.Themes.First(ct => ct.Name != themeName);
    testeeService.SelectedTheme = newTheme;
    _appResources.MergedDictionaries.Should().Contain(rd => rd.Source.ToString().EndsWith(newTheme.XamlFileName));
    _settingsService.Received(1).Theme = newTheme.Name;
    _themeChangedEvent.Received(1).Publish();
    testeeService.SelectedTheme.Should().Be(newTheme);
  }
}
