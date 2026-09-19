using JustClipboardManager.Application.Services;
using Windows.ApplicationModel.DataTransfer;

namespace JustClipboardManager.Services;

internal interface IPluginsService
{
  void LoadPlugins();
}


internal class PluginsService : PluginsService<DataPackageView>, IPluginsService
{

}
