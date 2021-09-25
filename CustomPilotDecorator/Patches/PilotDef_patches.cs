using BattleTech;
using BattleTech.Data;
using Harmony;
using System;
using System.Collections.Generic;

namespace CustomPilotDecorator {
  [HarmonyPatch(typeof(PilotDef))]
  [HarmonyPatch("DependenciesLoaded")]
  [HarmonyPatch(MethodType.Normal)]
  [HarmonyPatch(new Type[] { typeof(uint) })]
  public static class PilotDef_DependenciesLoaded {
    public static void Postfix(PilotDef __instance, uint loadWeight, ref bool __result) {
      try {
        Log.TWL(0, "PilotDef.DependenciesLoaded " + __instance.Description.Id);
        if (__result == false) { return; }
        HashSet<PilotDecorationDef> decorations = __instance.getDecorations();
        foreach (PilotDecorationDef decoration in decorations) {
          if (decoration.DataManager == null) { decoration.DataManager = __instance.DataManager; }
          if (decoration.DependenciesLoaded(loadWeight) == false) {
            __result = false;
            break;
          }
        }
      } catch (Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
    }
  }
  [HarmonyPatch(typeof(PilotDef))]
  [HarmonyPatch("GatherDependencies")]
  [HarmonyPatch(MethodType.Normal)]
  [HarmonyPatch(new Type[] { typeof(DataManager), typeof(DataManager.DependencyLoadRequest), typeof(uint) })]
  public static class PilotDef_GatherDependencies {
    public static void Postfix(PilotDef __instance, DataManager dataManager, DataManager.DependencyLoadRequest dependencyLoad, uint activeRequestWeight) {
      try {
        Log.TWL(0, "PilotDef.GatherDependencies " + __instance.Description.Id);
        HashSet<PilotDecorationDef> decorations = __instance.getDecorations();
        foreach (PilotDecorationDef decoration in decorations) {
          decoration.GatherDependencies(dataManager, dependencyLoad, activeRequestWeight);
        }
      } catch (Exception e) {
        Log.TWL(0, e.ToString(), true);
      }
    }
  }

}