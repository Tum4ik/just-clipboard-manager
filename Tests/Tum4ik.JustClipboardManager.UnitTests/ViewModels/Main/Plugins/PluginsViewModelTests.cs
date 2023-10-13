using Prism.Events;
using Prism.Regions;
using Tum4ik.JustClipboardManager.Constants;
using Tum4ik.JustClipboardManager.PluginDevKit.Events;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Main.Plugins;

namespace Tum4ik.JustClipboardManager.UnitTests.ViewModels.Main.Plugins;
public class PluginsViewModelTests
{
  private readonly ITranslationService _translationService = Substitute.For<ITranslationService>();
  private readonly IEventAggregator _eventAggregator = Substitute.For<IEventAggregator>();
  private readonly IRegionManager _regionManager = Substitute.For<IRegionManager>();
  private readonly PluginsViewModel _testeeVm;

  public PluginsViewModelTests()
  {
    _eventAggregator.GetEvent<LanguageChangedEvent>().Returns(new LanguageChangedEvent());
    _testeeVm = new(_translationService, _eventAggregator, _regionManager);
  }


  [Theory]
  [InlineData("A view")]
  [InlineData("Other view")]
  [InlineData("Another View")]
  internal void NavigateCommandTest(string viewName)
  {
    _testeeVm.NavigateCommand.Execute(viewName);
    _regionManager.Received(1).RequestNavigate(RegionNames.MainDialogPluginsViewContent, viewName);
  }
}
