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
  PotentialConfigChangesAttack,
  Incompatibility,
  MissingPluginModuleType,
  TypesLoadingProblem,
  PluginModuleInstanceCreationProblem,

  OtherProblem
}
