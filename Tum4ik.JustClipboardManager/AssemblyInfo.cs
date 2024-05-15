using System.Runtime.CompilerServices;
using System.Windows.Markup;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Tum4ik.JustClipboardManager.UnitTests")]

#if DEBUG
[assembly: XmlnsDefinition("debug-mode", "Tum4ik.JustClipboardManager")]
#endif
