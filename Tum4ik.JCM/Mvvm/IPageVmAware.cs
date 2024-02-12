namespace Tum4ik.JustClipboardManager.Mvvm;
internal interface IPageVmAware<out TVm>
  where TVm : class
{
  TVm Vm { get; }
}
