using Prism.Events;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;
using Tum4ik.JustClipboardManager.Services;
using Tum4ik.JustClipboardManager.Services.Translation;
using Tum4ik.JustClipboardManager.ViewModels.Base;

namespace Tum4ik.JustClipboardManager.ViewModels.Main.Settings;
internal partial class SettingsGeneralViewModel : TranslationViewModel
{
  private readonly IShortcutService _shortcutService;
  private readonly ISettingsService _settingsService;

  public SettingsGeneralViewModel(ITranslationService translationService,
                                  IEventAggregator eventAggregator,
                                  IShortcutService shortcutService,
                                  ISettingsService settingsService)
    : base(translationService, eventAggregator)
  {
    _shortcutService = shortcutService;
    _settingsService = settingsService;
  }


  public bool AutoStartApplication
  {
    get => _shortcutService.Exists(Environment.SpecialFolder.Startup, out _);
    set
    {
      if (value)
      {
        _shortcutService.Create(Environment.SpecialFolder.Startup);
      }
      else
      {
        _shortcutService.Delete(Environment.SpecialFolder.Startup);
      }
      OnPropertyChanged();
    }
  }


  private int? _removeClipsPeriod;
  public int RemoveClipsPeriod
  {
    get => _removeClipsPeriod ??= _settingsService.RemoveClipsPeriod;
    set
    {
      if (value != _removeClipsPeriod)
      {
        _settingsService.RemoveClipsPeriod = value;
        _removeClipsPeriod = value;
        OnPropertyChanged();
      }
    }
  }


  public IReadOnlyCollection<PeriodType> RemoveClipsPeriodTypes { get; } = Enum.GetValues<PeriodType>().AsReadOnly();

  private PeriodType? _selectedRemoveClipsPeriodType;
  public PeriodType SelectedRemoveClipsPeriodType
  {
    get => _selectedRemoveClipsPeriodType ??= _settingsService.RemoveClipsPeriodType;
    set
    {
      if (value != _selectedRemoveClipsPeriodType)
      {
        _settingsService.RemoveClipsPeriodType = value;
        _selectedRemoveClipsPeriodType = value;
        OnPropertyChanged();
      }
    }
  }
}
