using System.IO;
using System.Windows.Media.Imaging;

namespace Tum4ik.JustClipboardManager.PluginDevKit.Utils;
public static class ImageUtils
{
  public static BitmapSource GetBitmapSourceFromBytes(byte[] bytes)
  {
    using var memoryStream = new MemoryStream(bytes);
    return BitmapFrame.Create(memoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
  }
}
