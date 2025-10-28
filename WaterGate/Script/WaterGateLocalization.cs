using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace WaterGateKai
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
                Debug.Log("[WaterGateKai] Localization initialized.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[WaterGateKai] Error in Localization.Initialize postfix: {e}");
            }
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class Db_Initialize_Patch
    {
        private static bool alreadyRegistered = false;

        public static void Postfix()
        {
            if (alreadyRegistered) return;
            alreadyRegistered = true;

            try
            {
                Debug.Log("[WaterGateKai] Db.Initialize Postfix running.");

                // 注册建筑到建造菜单
                ModUtil.AddBuildingToPlanScreen("Utilities", WaterGateDefs.WaterGate1x2);
                ModUtil.AddBuildingToPlanScreen("Utilities", WaterGateDefs.WaterGate1x3);
                ModUtil.AddBuildingToPlanScreen("Utilities", WaterGateDefs.WaterGate1x4);
                ModUtil.AddBuildingToPlanScreen("Utilities", WaterGateDefs.GelGate);

                // 注册到科技树 - 修正科技ID
                AddToTechSafe("LiquidPiping", WaterGateDefs.WaterGate1x2);           // 液体管道
                AddToTechSafe("AdvancedDistillation", WaterGateDefs.GelGate);        // 高级蒸馏
                AddToTechSafe("RenewableEnergy", WaterGateDefs.WaterGate1x3);        // 可再生能源
                AddToTechSafe("RenewableEnergy", WaterGateDefs.WaterGate1x4);        // 可再生能源

                Debug.Log("[WaterGateKai] Buildings registered successfully (Db.Initialize).");

                // 加载翻译
                var translationDict = LoadTranslations();

                // 手动刷新关键 LocString
                RefreshLocStrings(new string[]
                {
                    "STRINGS.BUILDINGS.PREFABS.PIAPIWATERGATE1X2.NAME",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIWATERGATE1X2.DESC",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIWATERGATE1X2.EFFECT",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIWATERGATE1X3.NAME",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIWATERGATE1X3.DESC",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIWATERGATE1X3.EFFECT",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIWATERGATE1X4.NAME",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIWATERGATE1X4.DESC",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIWATERGATE1X4.EFFECT",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIGELGATE.NAME",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIGELGATE.DESC",
                    "STRINGS.BUILDINGS.PREFABS.PIAPIGELGATE.EFFECT"
                }, translationDict);

            }
            catch (Exception e)
            {
                Debug.LogError($"[WaterGateKai] Error in Db.Initialize postfix: {e}");
            }
        }

        private static void AddToTechSafe(string techId, string buildingId)
        {
            var tech = Db.Get().Techs.TryGet(techId);
            if (tech != null)
            {
                if (!tech.unlockedItemIDs.Contains(buildingId))
                {
                    tech.unlockedItemIDs.Add(buildingId);
                    Debug.Log($"[WaterGateKai] Added {buildingId} to tech '{techId}'.");
                }
                else
                {
                    Debug.Log($"[WaterGateKai] {buildingId} already in tech '{techId}'.");
                }
            }
            else
            {
                Debug.LogWarning($"[WaterGateKai] Tech '{techId}' not found. Skipped unlock for {buildingId}.");
            }
        }

        private static Dictionary<string, string> LoadTranslations()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string translationsDir = Path.Combine(directoryName, "translations");

            Localization.Locale locale = Localization.GetLocale();
            string localeCode = locale != null ? locale.Code : "en";
            string poPath = Path.Combine(translationsDir, localeCode + ".po");

            var dict = new Dictionary<string, string>();

            if (!File.Exists(poPath))
            {
                Debug.Log($"[WaterGateKai] Translation file not found: {poPath}");
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

                Debug.Log($"[WaterGateKai] OverloadStrings executed for {localeCode}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[WaterGateKai] Exception while loading translations: {e}");
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
                        Debug.Log($"[WaterGateKai] Refreshed LocString for: {key}");
                    }
                    else
                    {
                        Debug.LogWarning($"[WaterGateKai] Translation not found for key: {key}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[WaterGateKai] LocString key not found: {key}");
                }
            }
        }
    }
}