using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class SettingsPasteWindowViewModel : TranslationViewModel
{
  private readonly ISettingsService _settingsService;

  public SettingsPasteWindowViewModel(ITranslationService translationService,
                                      IEventAggregator eventAggregator,
                                      ISettingsService settingsService)
    : base(translationService, eventAggregator)
  {
    _settingsService = settingsService;
  }


  public IReadOnlyCollection<PasteWindowSnappingType> SnappingTypes { get; }
    = Enum.GetValues<PasteWindowSnappingType>().AsReadOnly();

  public IReadOnlyCollection<PasteWindowSnappingDisplayCorner> SnappingDisplayCorners { get; }
    = Enum.GetValues<PasteWindowSnappingDisplayCorner>().AsReadOnly();


  public PasteWindowSnappingType SnappingType
  {
    get => _settingsService.PasteWindowSnappingType;
    set => _settingsService.PasteWindowSnappingType = value;
  }


  public PasteWindowSnappingDisplayCorner SnappingDisplayCorner
  {
    get => _settingsService.PasteWindowSnappingDisplayCorner;
    set => _settingsService.PasteWindowSnappingDisplayCorner = value;
  }


  public int WindowWidth
  {
    get => _settingsService.PasteWindowWidth;
    set {
      _settingsService.PasteWindowWidth = value;
      SetDefaultWidthCommand.NotifyCanExecuteChanged();
    }
  }

  public int WindowMinWidth => _settingsService.PasteWindowMinWidth;


  public int WindowHeight
  {
    get => _settingsService.PasteWindowHeight;
    set
    {
      _settingsService.PasteWindowHeight = value;
      SetDefaultHeightCommand.NotifyCanExecuteChanged();
    }
  }

  public int WindowMinHeight => _settingsService.PasteWindowMinHeight;


  [RelayCommand(CanExecute = nameof(CanExecuteSetDefaultWidth))]
  private void SetDefaultWidth()
  {
    WindowWidth = _settingsService.PasteWindowDefaultWidth;
    OnPropertyChanged(nameof(WindowWidth));
  }

  private bool CanExecuteSetDefaultWidth()
  {
    return WindowWidth != _settingsService.PasteWindowDefaultWidth;
  }


  [RelayCommand(CanExecute = nameof(CanExecuteSetDefaultHeight))]
  private void SetDefaultHeight()
  {
    WindowHeight = _settingsService.PasteWindowDefaultHeight;
    OnPropertyChanged(nameof(WindowHeight));
  }

  private bool CanExecuteSetDefaultHeight()
  {
    return WindowHeight != _settingsService.PasteWindowDefaultHeight;
  }
}
