using System.Windows;
using Prism.Events;
using Prism.Ioc;
using Tum4ik.JustClipboardManager.Events;
using Tum4ik.JustClipboardManager.PluginDevKit.Theming;
using Tum4ik.JustClipboardManager.Services.Translation;

namespace Tum4ik.JustClipboardManager.Controls;

/// <summary>
/// Interaction logic for PluginCard.xaml
/// </summary>
public partial class PluginCard
{
  private static IEventAggregator s_eventAggregator = ((App) App.Current).Container.Resolve<IEventAggregator>();
  private static ITranslationService s_translate = ((App) App.Current).Container.Resolve<ITranslationService>();

  public PluginCard()
  {
    InitializeComponent();
    s_eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(UpdateByTranslation);
  }


  private void This_Loaded(object sender, RoutedEventArgs e)
  {
    UpdateByTranslation();
  }


  private void UpdateByTranslation()
  {
    _by.Text = s_translate["by"];
  }


  internal static readonly DependencyProperty PluginNameProperty = DependencyProperty.Register(
    nameof(PluginName), typeof(string), typeof(PluginCard)
  );
  internal string PluginName
  {
    get => (string) GetValue(PluginNameProperty);
    set => SetValue(PluginNameProperty, value);
  }


  internal static readonly DependencyProperty VersionProperty = DependencyProperty.Register(
    nameof(Version), typeof(Version), typeof(PluginCard)
  );
  internal Version Version
  {
    get => (Version) GetValue(VersionProperty);
    set => SetValue(VersionProperty, value);
  }


  internal static readonly DependencyProperty AuthorProperty = DependencyProperty.Register(
    nameof(Author), typeof(string), typeof(PluginCard)
  );
  internal string? Author
  {
    get => (string) GetValue(AuthorProperty);
    set => SetValue(AuthorProperty, value);
  }


  internal static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
    nameof(Description), typeof(string), typeof(PluginCard)
  );
  internal string? Description
  {
    get => (string) GetValue(DescriptionProperty);
    set => SetValue(DescriptionProperty, value);
  }


  internal static readonly DependencyProperty IsPluginEnabledProperty = DependencyProperty.Register(
    nameof(IsPluginEnabled), typeof(bool?), typeof(PluginCard), new((d, e) =>
    {
      var thisCard = (PluginCard) d;
      if (e.NewValue is true)
      {
        thisCard.SetResourceReference(BackgroundProperty, AppColors.CardBackgroundBrush);
      }
      else
      {
        thisCard.SetResourceReference(BackgroundProperty, AppColors.CardDisabledBackgroundBrush);
      }
    })
  );
  internal bool? IsPluginEnabled
  {
    get => (bool?) GetValue(IsPluginEnabledProperty);
    set => SetValue(IsPluginEnabledProperty, value);
  }


  public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
    nameof(Footer), typeof(FrameworkElement), typeof(PluginCard)
  );
  public FrameworkElement? Footer
  {
    get => (FrameworkElement) GetValue(FooterProperty);
    set => SetValue(FooterProperty, value);
  }
}
