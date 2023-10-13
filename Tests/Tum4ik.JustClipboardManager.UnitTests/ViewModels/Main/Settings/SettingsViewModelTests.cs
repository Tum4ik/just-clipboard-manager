using Prism.Events;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.PluginDevKit.Events;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Main.Settings;

namespace Tum4ik.JustClipboardManager.UnitTests.ViewModels.Main.Settings;
public class SettingsViewModelTests
{
  private readonly ITranslationService _translationService = Substitute.For<ITranslationService>();
  private readonly IEventAggregator _eventAggregator = Substitute.For<IEventAggregator>();
  private readonly IRegionManager _regionManager = Substitute.For<IRegionManager>();
  private readonly SettingsViewModel _testeeVm;


  public SettingsViewModelTests()
  {
    _eventAggregator.GetEvent<LanguageChangedEvent>().Returns(new LanguageChangedEvent());
    _testeeVm = new(_regionManager, _translationService, _eventAggregator);
  }


  [Theory]
  [InlineData("A view")]
  [InlineData("Other view")]
  [InlineData("Another View")]
  internal void NavigateCommandTest(string viewName)
  {
    _testeeVm.NavigateCommand.Execute(viewName);
    _regionManager.Received(1).RequestNavigate(RegionNames.MainDialogSettingsViewContent, viewName);
  }
}
