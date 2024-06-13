namespace Tum4ik.JustClipboardManager.Services.Plugins;
internal enum PluginInstallationResult
{
  Success,

  CancelledByUser,
  InternetConnectionProblem,
  EmptyArchive,
  ExceededArchiveEntriesCount,
  AbnormalArchiveCompressionRatio,
  ExceededUncompressedArchiveSize,
  Incompatibility,
  MissingPluginModuleType,
  TypesLoadingProblem,
  PluginModuleInstanceCreationProblem,

  OtherProblem
}
