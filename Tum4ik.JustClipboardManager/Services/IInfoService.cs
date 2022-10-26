using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IInfoService
{
  string GetInformationalVersion();
  Version GetVersion();
}
