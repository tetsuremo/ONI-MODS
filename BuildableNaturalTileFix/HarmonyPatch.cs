using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace BuildableNaturalTileFix
{
    [HarmonyPatch(typeof(Localization), "Initialize")]
    public static class Localization_Initialize_Patch
    {
        public static void Postfix()
        {
            try
            {
                Localization.RegisterForTranslation(typeof(STRINGS));
                LocString.CreateLocStringKeys(typeof(STRINGS), null);
            }
            catch (Exception e)
            {
                Debug.LogError($"[BNTFix] Error in Localization.Initialize postfix: {e}");
            }
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class Db_Initialize_Patch
    {
        private static bool alreadyRegistered = false;
        private const string BUILDING_ID = "BUILDABLENATURALTILEFIX_NATURALTILE";

        public static void Postfix()
        {
            if (alreadyRegistered) return;
            alreadyRegistered = true;

            try
            {
                Debug.Log("[BNTFix] Db.Initialize Postfix running.");

                // 注册建筑
                ModUtil.AddBuildingToPlanScreen("Base", BUILDING_ID, "MeshTile");
                Tech tech = Db.Get().Techs.Get("FarmingTech");
                if (tech != null && !tech.unlockedItemIDs.Contains(BUILDING_ID))
                    tech.unlockedItemIDs.Add(BUILDING_ID);

                Debug.Log("[BNTFix] Building registered successfully (Db.Initialize).");

                // 加载翻译
                var translationDict = LoadTranslations();

                // 创建 LocString
                LocString.CreateLocStringKeys(typeof(STRINGS), null);
                Debug.Log("[BNTFix] LocString keys created after OverloadStrings.");

                // 手动刷新关键 LocString
                RefreshLocStrings(new string[]
                {
                    "STRINGS.BUILDINGS.PREFABS.BUILDABLENATURALTILEFIX_NATURALTILE.NAME",
                    "STRINGS.BUILDINGS.PREFABS.BUILDABLENATURALTILEFIX_NATURALTILE.DESC",
                    "STRINGS.BUILDINGS.PREFABS.BUILDABLENATURALTILEFIX_NATURALTILE.EFFECT"
                }, translationDict);

            }
            catch (Exception e)
            {
                Debug.LogError($"[BNTFix] Error in Db.Initialize postfix: {e}");
            }
        }

        private static Dictionary<string, string> LoadTranslations()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string translationsDir = Path.Combine(directoryName, "translations");

            Localization.Locale locale = Localization.GetLocale();
            string localeCode = (locale != null) ? locale.Code : "en";
            string poPath = Path.Combine(translationsDir, localeCode + ".po");

            var dict = new Dictionary<string, string>();

            if (!File.Exists(poPath))
            {
                return dict;
            }

            try
            {
                var translations = Localization.LoadStringsFile(poPath, false);
                Localization.OverloadStrings(translations);

                foreach (var kvp in translations)
                {
                    dict[kvp.Key] = kvp.Value;
                }

                Debug.Log($"[BNTFix] OverloadStrings executed.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[BNTFix] Exception while loading translations: {e}");
            }

            return dict;
        }

        private static void RefreshLocStrings(string[] keys, Dictionary<string, string> translationDict)
        {
            foreach (var key in keys)
            {
                if (Strings.TryGet(key, out var entry))
                {
                    if (translationDict.TryGetValue(key, out var translated))
                    {
                        entry.String = translated; // 强制刷新 LocString
                    }
                    else
                    {
                        Debug.LogWarning($"[BNTFix] Translation not found for key: {key}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[BNTFix] LocString key not found: {key}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public static class BuildingComplete_OnSpawn_Patch
    {
        public static void Postfix(BuildingComplete __instance)
        {
            if (__instance == null) return;

            try
            {
                var kpid = __instance.GetComponent<KPrefabID>();
                if (kpid == null) return;

                if (kpid.PrefabTag.ToString() != "BUILDABLENATURALTILEFIX_NATURALTILE")
                    return;

                int cell = Grid.PosToCell(__instance.gameObject);
                var primary = __instance.GetComponent<PrimaryElement>();
                if (primary != null && primary.Element != null && Grid.IsValidCell(cell))
                {
                    SimHashes elementID = primary.ElementID;
                    float mass = primary.Mass;
                    float temperature = primary.Temperature;

                    SimMessages.ReplaceAndDisplaceElement(cell, elementID, null, mass, temperature, byte.MaxValue, 0, -1);
                }

                UnityEngine.Object.Destroy(__instance.gameObject);
            }
            catch (Exception e)
            {
                Debug.LogError($"[BNTFix] Error in BuildingComplete.OnSpawn: {e}");
            }
        }
    }
}
