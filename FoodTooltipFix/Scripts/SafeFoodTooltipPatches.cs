using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.PatchManager;
using System.Collections.Generic;
using UnityEngine;

namespace PeterHan.FoodTooltip
{
    public sealed class SafeFoodTooltipPatches : KMod.UserMod2
    {
        [PLibMethod(RunAt.OnEndGame)]
        internal static void CleanupCache()
        {
            FoodRecipeCache.DestroyInstance();
        }

        [PLibMethod(RunAt.OnStartGame)]
        internal static void InitCache()
        {
            FoodRecipeCache.CreateInstance();
        }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PLocalization().Register();
            new PPatchManager(harmony).RegisterPatchClass(typeof(SafeFoodTooltipPatches));
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }

        [HarmonyPatch(typeof(CreatureCalorieMonitor.Def), nameof(CreatureCalorieMonitor.Def.GetDescriptors))]
        public static class CreatureCalorieMonitor_Def_GetDescriptors_Patch
        {
            internal static void Postfix(GameObject obj, List<Descriptor> __result)
            {
                try
                {
                    if (__result != null && obj != null)
                        SafeFoodTooltipUtils.AddCritterDescriptors(obj, __result);
                }
                catch (System.Exception e)
                {
                    PUtil.LogWarning($"[SafeFoodTooltip] Postfix failed: {e}");
                }
            }
        }

        [HarmonyPatch(typeof(Crop), nameof(Crop.InformationDescriptors))]
        public static class Crop_InformationDescriptors_Patch
        {
            internal static void Postfix(Crop __instance, List<Descriptor> __result)
            {
                if (__result != null)
                    SafeFoodTooltipUtils.AddCropDescriptors(__instance, __result);
            }
        }

        [HarmonyPatch(typeof(MeterScreen_Rations), "OnTooltip")]
        public static class MeterScreenRations_Refresh_Patch
        {
            internal static void Postfix(MeterScreen_ValueTrackerDisplayer __instance)
            {
                SafeFoodTooltipUtils.ShowFoodUseStats(__instance.Tooltip, __instance.ToolTipStyle_Property);
            }
        }

        [HarmonyPatch(typeof(SimpleInfoScreen), "OnPrefabInit")]
        public static class SimpleInfoScreen_OnPrefabInit_Patch
        {
            internal static void Postfix(SimpleInfoScreen __instance)
            {
                __instance.gameObject.AddOrGet<InfoScreenRefresher>();
            }
        }

        [HarmonyPatch(typeof(SimpleInfoScreen), "OnSelectTarget")]
        public static class SimpleInfoScreen_OnSelectTarget_Patch
        {
            internal static void Postfix(SimpleInfoScreen __instance, GameObject target)
            {
                if (__instance != null && __instance.TryGetComponent(out InfoScreenRefresher rf))
                    rf.OnSelectTarget(target);
            }
        }

        [HarmonyPatch(typeof(SimpleInfoScreen), nameof(SimpleInfoScreen.OnDeselectTarget))]
        public static class SimpleInfoScreen_OnDeselectTarget_Patch
        {
            internal static void Prefix(SimpleInfoScreen __instance, GameObject target)
            {
                if (__instance != null && __instance.TryGetComponent(out InfoScreenRefresher rf))
                    rf.OnDeselectTarget(target);
            }
        }
    }
}
