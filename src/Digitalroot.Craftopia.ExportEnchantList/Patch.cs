using Digitalroot.Valheim.Common;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Reflection;

namespace Digitalroot.Craftopia.ExportEnchantList
{
  [UsedImplicitly]
  public class Patch
  {
    [HarmonyPatch(typeof(Oc.OcUI_HomeScene), nameof(Oc.OcUI_HomeScene.Start))]
    // ReSharper disable once InconsistentNaming
    public class PatchOcUI_HomeScene
    {
      [UsedImplicitly]
      [HarmonyPostfix]
      [HarmonyPriority(Priority.Low)]
      public static void Postfix()
      {
        try
        {
          Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");

          Main.Instance.OnOcUI_HomeSceneStart();
        }
        catch (Exception e)
        {
          Log.Error(Main.Instance, e);
        }
      }
    }
  }
}
