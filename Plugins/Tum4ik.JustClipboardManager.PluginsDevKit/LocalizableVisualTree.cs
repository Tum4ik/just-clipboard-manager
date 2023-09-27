using System.ComponentModel;
using System.Windows.Controls;
using Prism.Events;
using Prism.Ioc;
using Tum4ik.JustClipboardManager.PluginDevKit.Events;
using Tum4ik.JustClipboardManager.PluginDevKit.Services;

namespace Tum4ik.JustClipboardManager.PluginDevKit;
public abstract class LocalizableVisualTree : Decorator, INotifyPropertyChanged
{
  private static IPluginTranslationService s_translate = ContainerLocator.Container.Resolve<IPluginTranslationService>();
  private static IEventAggregator s_eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();

  public IPluginTranslationService Translate { get; } = s_translate;

  protected LocalizableVisualTree()
  {
    DataContext = this;
    Loaded += (s, e) => s_eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(LanguageChanged);
    Unloaded += (s, e) => s_eventAggregator.GetEvent<LanguageChangedEvent>().Unsubscribe(LanguageChanged);
  }


  public event PropertyChangedEventHandler? PropertyChanged;


  private void LanguageChanged()
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Translate)));
  }
}
