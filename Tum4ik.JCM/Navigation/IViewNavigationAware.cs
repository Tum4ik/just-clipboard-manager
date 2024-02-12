namespace Tum4ik.JustClipboardManager.Navigation;

internal interface IViewNavigationAware
{
  void OnNavigatedTo(Dictionary<string, object> parameters);
  void OnNavigatedFrom();
}
