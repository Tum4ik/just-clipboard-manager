using Tum4ik.JustClipboardManager.Mvvm;
using Tum4ik.JustClipboardManager.Navigation;
using Tum4ik.JustClipboardManager.Views;

namespace Tum4ik.JustClipboardManager.Controls;

internal sealed class AppNavigationView : NavigationView
{
  private readonly Frame _frame;

  public AppNavigationView()
  {
    _frame = new Frame();
    _frame.Navigated += Frame_Navigated;
    Content = _frame;
    ItemInvoked += AppNavigationView_ItemInvoked;
  }


  private void Frame_Navigated(object sender, NavigationEventArgs e)
  {
    var vmNavigationAware = (e.SourcePageType as IPageVmAware<dynamic>)?.Vm as IViewNavigationAware;
    if (vmNavigationAware is not null)
    {
      vmNavigationAware.OnNavigatedTo(e.Parameter as Dictionary<string, object> ?? []);
    }
  }


  private void AppNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
  {
    if (args.InvokedItemContainer is PageNavigationViewItem navigationViewItem
      && navigationViewItem.NavigateTo is not null
      && AppPages.TryGetPageType(navigationViewItem.NavigateTo.Value, out var pageType))
    {
      _frame.Navigate(pageType);
    }
  }
}
