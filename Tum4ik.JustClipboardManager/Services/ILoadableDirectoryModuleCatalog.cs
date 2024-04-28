using Prism.Modularity;
using HarmonyLib;

namespace Tum4ik.JustClipboardManager.Services;
internal interface ILoadableDirectoryModuleCatalog : IModuleCatalog
{
  void Load();
}


internal class LoadableDirectoryModuleCatalog : DirectoryModuleCatalog, ILoadableDirectoryModuleCatalog
{
  private readonly Harmony _harmony;

  public LoadableDirectoryModuleCatalog(Harmony harmony)
  {
    _harmony = harmony;
    OverrideModuleInfoCreation();
  }


  private void OverrideModuleInfoCreation()
  {
    var sourceClass = AccessTools.TypeByName("Prism.Modularity.DirectoryModuleCatalog+InnerModuleInfoLoader");
    var original_CreateModuleInfo = AccessTools.Method(sourceClass, "CreateModuleInfo");
    var postfix_CreateModuleInfo = SymbolExtensions.GetMethodInfo<ModuleInfo, ModuleInfo>(
      moduleInfo => Postfix_CreateModuleInfo(moduleInfo)
    );
    _harmony.Patch(original_CreateModuleInfo, postfix: new HarmonyMethod(postfix_CreateModuleInfo));
  }


  private static ModuleInfo Postfix_CreateModuleInfo(ModuleInfo moduleInfo)
  {
    if (moduleInfo.ModuleName == "TextPlugin")
    {
      return moduleInfo;
    }

    moduleInfo.InitializationMode = InitializationMode.OnDemand;
    return moduleInfo;
  }
}
