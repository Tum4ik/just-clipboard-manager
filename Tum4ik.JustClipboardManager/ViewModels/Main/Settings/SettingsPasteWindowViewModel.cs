using Prism.Events;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal class SettingsPasteWindowViewModel : TranslationViewModel
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


  private PasteWindowSnappingType? _snappingType;
  public PasteWindowSnappingType SnappingType
  {
    get => _snappingType ??= _settingsService.PasteWindowSnappingType;
    set
    {
      if (value != _snappingType)
      {
        _settingsService.PasteWindowSnappingType = value;
        _snappingType = value;
        OnPropertyChanged();
      }
    }
  }


  private PasteWindowSnappingDisplayCorner? _snappingDisplayCorner;
  public PasteWindowSnappingDisplayCorner SnappingDisplayCorner
  {
    get => _snappingDisplayCorner ??= _settingsService.PasteWindowSnappingDisplayCorner;
    set
    {
      if (value != _snappingDisplayCorner)
      {
        _settingsService.PasteWindowSnappingDisplayCorner = value;
        _snappingDisplayCorner = value;
        OnPropertyChanged();
      }
    }
  }
}
