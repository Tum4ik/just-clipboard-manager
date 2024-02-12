using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager.Controls;

internal class PageNavigationViewItem : NavigationViewItem
{
  public static readonly DependencyProperty NavigateToProperty = DependencyProperty.Register(
    nameof(NavigateTo), typeof(AppPage), typeof(AppBarNavigationViewItem), new(null)
  );
  public AppPage? NavigateTo
  {
    get => (AppPage) GetValue(NavigateToProperty);
    set => SetValue(NavigateToProperty, value);
  }
}
