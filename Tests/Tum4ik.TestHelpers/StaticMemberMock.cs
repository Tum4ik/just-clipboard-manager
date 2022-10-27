using System.Linq.Expressions;
using HarmonyLib;

namespace Tum4ik.TestHelpers;

public static class StaticMemberMock
{
  public static void PropertyGetter(Type staticClassType, string staticPropertyName, Expression<Action> expression)
  {
    var harmony = new Harmony(Guid.NewGuid().ToString());
    var getter = AccessTools.PropertyGetter(staticClassType, staticPropertyName);
    var postfix = SymbolExtensions.GetMethodInfo(expression);
    harmony.Patch(getter, postfix: new HarmonyMethod(postfix));
  }
}

